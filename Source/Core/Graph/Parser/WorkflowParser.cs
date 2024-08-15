﻿namespace DotNetToolbox.Graph.Parser;

public sealed class WorkflowParser {
    private readonly IEnumerator<Token> _tokens;
    private readonly WorkflowBuilder _builder;
    private Token _currentToken;

    private WorkflowParser(IEnumerable<Token> tokens, IServiceProvider services) {
        _tokens = tokens.GetEnumerator();
        _builder = new WorkflowBuilder(services);
        NextToken();
    }

    public static INode? Parse(IEnumerable<Token> tokens, IServiceProvider services) {
        var parser = new WorkflowParser(tokens, services);
        return parser.Process();
    }

    private INode? Process() {
        ParseStatements(_builder);
        return _builder.Build();
    }

    private void ParseStatements(IWorkflowBuilder builder) {
        var indentColumn = _currentToken.Column;
        while (_currentToken.Type is not TokenType.EOF && _currentToken.Column >= indentColumn)
            ParseStatement(builder);
    }

    private void ParseStatement(IWorkflowBuilder builder) {
        switch (_currentToken.Type) {
            case TokenType.Identifier:
                ParseAction(builder);
                break;
            case TokenType.If:
                ParseIf(builder);
                break;
            case TokenType.Case:
                ParseCase(builder);
                break;
            case TokenType.Exit:
                ParseExit(builder);
                break;
            case TokenType.JumpTo:
                ParseJumpTo(builder);
                break;
            case TokenType.EOL: // Empty line
                NextToken();
                break;
            default:
                throw new InvalidOperationException($"Unexpected token: {_currentToken.Type}");
        }
    }

    private void ParseAction(IWorkflowBuilder builder) {
        var name = GetValueFrom(TokenType.Identifier);
        var tag = GetValueOrDefaultFrom(TokenType.Tag);
        var label = GetValueOrDefaultFrom(TokenType.Label) ?? name;
        builder.Do(tag!,
                   ctx => BuildAction(ctx, name),
                   label);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);
    }

    private void ParseIf(IWorkflowBuilder builder) {
        Ensure(TokenType.If);
        var predicate = ParsePredicate();
        var tag = GetValueOrDefaultFrom(TokenType.Tag);
        var label = GetValueOrDefaultFrom(TokenType.Label);
        builder.If(tag!,
                   ctx => BuildPredicate(ctx, predicate),
                   b => b.IsTrue(ParseThen)
                         .IsFalse(ParseElse),
                   label);
    }

    private string ParsePredicate() {
        var condition = new StringBuilder();
        while (_currentToken.Type is not TokenType.Tag and not TokenType.Label and not TokenType.EOL and not TokenType.Then) {
            condition.Append(_currentToken.Value);
            condition.Append(' ');
            NextToken();
        }
        return condition.ToString().Trim();
    }

    private void ParseThen(IWorkflowBuilder builder) {
        Allow(TokenType.Then);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);
        ParseStatements(builder);
    }

    private void ParseElse(IWorkflowBuilder builder) {
        if (!Has(TokenType.Else))
            return;
        Ensure(TokenType.Else);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);
        ParseStatements(builder);
    }

    private void ParseCase(IWorkflowBuilder builder) {
        Ensure(TokenType.Case);
        var selector = GetValueFrom(TokenType.Identifier);
        var tag = GetValueOrDefaultFrom(TokenType.Tag);
        var label = GetValueOrDefaultFrom(TokenType.Label);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);

        builder.Case(tag!,
                     ctx => BuildSelector(ctx, selector),
                     ParseCaseOptions,
                     label);
    }

    private void ParseCaseOptions(CaseNodeBuilder branches) {
        var count = 0;
        while (TryParseCaseOption(branches)) count++;
        if (count == 0) Forbid(TokenType.Otherwise);
        ParseOtherwise(branches);
    }

    private bool TryParseCaseOption(CaseNodeBuilder branches) {
        if (!Has(TokenType.Is)) return false;
        Ensure(TokenType.Is);
        var caseValue = GetValueFrom(TokenType.String);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);
        branches.Is(caseValue, ParseStatements);
        return true;
    }

    private void ParseOtherwise(CaseNodeBuilder branches) {
        var indentColumn = _currentToken.Column;
        if (!Has(TokenType.Otherwise)) return;
        Ensure(TokenType.Otherwise);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);
        branches.Otherwise(ParseStatements);
        Forbid(TokenType.Otherwise);
    }

    private void ParseExit(IWorkflowBuilder builder) {
        Ensure(TokenType.Exit);
        var number = GetValueOrDefaultFrom(TokenType.Number) ?? "0"; // if number not found default to 0
        var exitCode = int.Parse(number);
        var tag = GetValueOrDefaultFrom(TokenType.Tag);
        var label = GetValueOrDefaultFrom(TokenType.Label);
        builder.Exit(tag, exitCode, label);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);
    }

    private void ParseJumpTo(IWorkflowBuilder builder) {
        Ensure(TokenType.JumpTo);
        var target = GetValueFrom(TokenType.Identifier);
        builder.JumpTo(target);
        Ensure(TokenType.EOL);
        AllowMany(TokenType.Indent);
    }

    // This is a simplified implementation.
    // In a real-world scenario, you might want to use a more sophisticated expression evaluator.
    private Action<Context> BuildAction(Context ctx, string action) {
        var result = ctx.TryGetValue(action, out var value)
                   && value is Action<Context> execute
                        ? execute
                        : _ => { };
        NextToken();
        return result;
    }

    // This is a simplified implementation.
    // In a real-world scenario, you might want to use a more sophisticated expression evaluator.
    private bool BuildPredicate(Context ctx, string condition) {
        var result = ctx.TryGetValue(condition, out var value)
                    && value is bool boolValue
                    && boolValue;
        NextToken();
        return result;
    }

    // This is a simplified implementation.
    // In a real-world scenario, you might want to use a more sophisticated expression evaluator.
    private string BuildSelector(Context ctx, string selector) {
        var result = ctx.TryGetValue(selector, out var value)
            ? value?.ToString() ?? string.Empty
            : string.Empty;
        NextToken();
        return result;
    }

    private bool Has(TokenType type) {
        if (_currentToken.Type != type) return false;
        return true;
    }

    private void Allow(TokenType type) {
        if (_currentToken.Type != type) return;
        NextToken();
    }

    private void AllowMany(TokenType type) {
        while (_currentToken.Type == type)
            NextToken();
    }

    private void Ensure(TokenType type) {
        if (_currentToken.Type != type)
            throw new InvalidOperationException($"Expected {type}, but got {_currentToken.Type}");
        NextToken();
    }

    private void Forbid(TokenType type) {
        if (_currentToken.Type == type)
            throw new InvalidOperationException($"{_currentToken.Type} is not allowed.");
    }

    private string GetValueFrom(TokenType type) {
        if (_currentToken.Type != type)
            throw new InvalidOperationException($"Expected token of type '{type}', but found '{_currentToken.Type}'");
        var token = _currentToken;
        NextToken();
        return token.Value ?? throw new InvalidOperationException($"The value for '{type}' token is not set.");
    }

    private string? GetValueOrDefaultFrom(TokenType type) {
        if (_currentToken.Type != type)
            return null;
        var token = _currentToken;
        NextToken();
        return token.Value;
    }

    [MemberNotNull(nameof(_currentToken))]
    private void NextToken()
        => _currentToken = _tokens.MoveNext()
            ? _tokens.Current
            : new Token(TokenType.Exit, 0, 0, string.Empty);
}
