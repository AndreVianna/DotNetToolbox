﻿namespace DotNetToolbox.Graph.Parser;

// # this is the line 1 of a comment
// # this is the line 2 of a comment
// Initialize
// :Label1
// DoSomething [This is a node description] # this is a comment
//   :Label2
//   IF CheckCondition [Condiftion description]
//     THEN
//       TrueAction1 [Update X]
//       TrueAction2
//       EXIT 1 "Alternative Exit"
//     ELSE
//       FalseAction # this is another comment
// WHEN SelectPath  [Path1 description]
//   IS "Path 1"
//     Path1Action
//     GOTO 0
//   IS "Path 2" [Path2 description]
//     :Label3
//     Path2Action1
//     Path2Action2
//     IF CheckAnotherCondition
//       THEN
//         Path2Action2TrueAction [Update X]
//   OTHERWISE
//     GOTO Label2
// DoSomethingElse
// DoSomething
// EXIT

public class WorkflowLexer {
    private const char _descriptionStart = '[';
    private const char _descriptionEnd = ']';
    private const char _labelPrefix = ':';
    private const char _stringDelimiter = '"';
    private const char _space = ' ';
    private const char _tab = '\t';
    private const char _commentStart = '#';

    private static readonly char[] _separators = [_space, _tab];

    private readonly string[] _lines;

    private int _currentLine;
    private int _currentColumn;
    private int _currentIndent;

    public WorkflowLexer(string input) {
        input = input.Replace("\r\n", "\n") // windows EOL
                     .Replace("\r", "\n");  // Mac OS EOL
        _lines = input.Split('\n').Select(l => l.TrimEnd()).ToArray();
        _currentLine = 0;
        _currentColumn = 0;
        _currentIndent = 0;
    }

    public IEnumerable<Token> Tokenize() {
        var tokens = new List<Token>();
        foreach (var line in _lines) {
            if (string.IsNullOrEmpty(line)) {
                _currentLine++;
                continue;
            }
            tokens.AddRange(ProcessLine(line));
            _currentLine++;
        }
        return tokens;
    }

    private IEnumerable<Token> ProcessLine(string line) {
        _currentColumn = 0;
        var indentLevel = GetIndentLevel(line);
        if (indentLevel != 0) {
            yield return new Token(TokenType.Indent, indentLevel, _currentLine + 1, 1);
            _currentIndent = indentLevel;
            _currentColumn = indentLevel * 2;
            line = line[_currentColumn..];
        }
        if (line[0] == _space) {
            _currentColumn++;
            line = line[_currentColumn..];
        }

        foreach (var token in SplitLine(line)) {
            yield return TokenizeWord(token);
        }

        yield return new Token(TokenType.EOL, "\n", _currentLine + 1, _currentColumn + 1);
    }

    private static IEnumerable<string> SplitLine(string line) {
        var tokens = new List<string>();
        var currentToken = new StringBuilder();
        var inString = false;
        var inDescription = false;

        for (var i = 0; i < line.Length; i++) {
            switch (line[i]) {
                case _commentStart when !inString && !inDescription:
                    currentToken.Clear();
                    i = line.Length;
                    break;
                case _stringDelimiter:
                    inString = !inString;
                    currentToken.Append(line[i]);
                    break;
                case _descriptionStart when !inString:
                    if (currentToken.Length > 0) {
                        tokens.Add(currentToken.ToString());
                        currentToken.Clear();
                    }
                    inDescription = true;
                    currentToken.Append(line[i]);
                    break;
                case _descriptionEnd when inDescription:
                    inDescription = false;
                    currentToken.Append(line[i]);
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                    break;
                default:
                    if (_separators.Contains(line[i]) && !inString && !inDescription) {
                        if (currentToken.Length > 0) {
                            tokens.Add(currentToken.ToString());
                            currentToken.Clear();
                        }
                        break;
                    }
                    currentToken.Append(line[i]);
                    break;
            }
        }

        if (currentToken.Length > 0) {
            tokens.Add(currentToken.ToString());
        }

        return tokens;
    }

    private Token TokenizeWord(string word) {
        var tokenStart = _currentColumn + 1;
        _currentColumn += IsNotNull(word).Length;
        if (word.StartsWith(_descriptionStart) && word.EndsWith(_descriptionEnd)) {
            var value = word.TrimStart(_descriptionStart).TrimEnd(_descriptionEnd);
            return new Token(TokenType.Description, value, _currentLine + 1, tokenStart);
        }
        if (word.StartsWith(_labelPrefix)) {
            return new Token(TokenType.Label, word[1..], _currentLine + 1, tokenStart);
        }
        if (word.StartsWith(_stringDelimiter) && word.EndsWith(_stringDelimiter)) {
            var value = word.Trim(_stringDelimiter);
            return new Token(TokenType.String, value, _currentLine + 1, tokenStart);
        }
        return word.ToUpperInvariant() switch {
            "IF" => new Token(TokenType.If, word, _currentLine + 1, tokenStart),
            "WHEN" => new Token(TokenType.When, word, _currentLine + 1, tokenStart),
            "THEN" => new Token(TokenType.Then, word, _currentLine + 1, tokenStart),
            "ELSE" => new Token(TokenType.Else, word, _currentLine + 1, tokenStart),
            "IS" => new Token(TokenType.Is, word, _currentLine + 1, tokenStart),
            "OTHERWISE" => new Token(TokenType.Otherwise, word, _currentLine + 1, tokenStart),
            "EXIT" => new Token(TokenType.Exit, word, _currentLine + 1, tokenStart),
            "GOTO" => new Token(TokenType.Goto, word, _currentLine + 1, tokenStart),
            _ when int.TryParse(word, out _) => new Token(TokenType.Number, word, _currentLine + 1, tokenStart),
            _ => new Token(TokenType.Identifier, word, _currentLine + 1, tokenStart)
        };
    }

    private static int GetIndentLevel(string line) {
        var offset = 0;
        foreach (var c in line) {
            switch (c) {
                case _space:
                    offset++;
                    break;
                case _tab:
                    offset += 2;
                    break;
                default:
                    return offset / 2;
            }
        }
        return offset / 2;
    }
}