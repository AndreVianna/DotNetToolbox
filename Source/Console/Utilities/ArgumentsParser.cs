﻿namespace DotNetToolbox.ConsoleApplication.Utilities;

public static class ArgumentsParser {
    public static async Task<Result> Parse(IHasChildren parent, IReadOnlyList<string> arguments, CancellationToken ct) {
        var result = Success();
        for (var index = 0; index < arguments.Count; index++)
            (result, index) = await TrySetupChild(parent, arguments, index, ct);
        return EnsureAllRequiredParametersAreSet(parent, result);
    }

    private static async Task<(Result result, int index)> TrySetupChild(IHasChildren node, IReadOnlyList<string> arguments, int index, CancellationToken ct) {
        var child = FindChild(node, arguments[index]);
        return child switch {
            IFlag f => (await f.Read(node.Context, ct), index),
            IOption when index >= arguments.Count - 1 => (Invalid($"Missing value for option '{arguments[index]}'."), index + 1),
            IOption o => (await o.Read(arguments[++index], node.Context, ct), index),
            ICommand c => (await c.Set(arguments.Skip(++index).ToArray(), ct), index),
            _ => (await ReadParameters(node, arguments.Skip(index).ToArray(), ct), arguments.Count - 1),
        };
    }

    private static INode? FindChild(IHasChildren node, string token)
        => token.StartsWith('"')
               ? null
               : token.StartsWith('-')
                   ? token.StartsWith("--")
                         ? node.Children.FirstOrDefault(c => c.Name.Equals(token.TrimStart('-'), StringComparison.CurrentCultureIgnoreCase))
                         : node.Children.FirstOrDefault(c => c.Aliases.Contains(token.TrimStart('-')))
                   : node.Children.FirstOrDefault(c => c.Name.Contains(token, StringComparison.CurrentCultureIgnoreCase)
                                               || c.Aliases.Contains(token));

    private static async Task<Result> ReadParameters(IHasChildren node, IReadOnlyList<string> arguments, CancellationToken ct) {
        if (node.Parameters.Length == 0) return Invalid($"Unknown argument '{arguments[0]}'. For a list of available arguments use '--help'.");
        var index = 0;
        var result = Success();
        foreach (var parameter in node.Parameters) {
            if (index >= arguments.Count) break;
            result += arguments[index].StartsWith('-')
                ? Invalid($"Unknown argument '{arguments[index]}'. For a list of available arguments use '--help'.")
                : await parameter.Read(arguments[index], node.Context, ct);
            index++;
        }

        return result;
    }

    private static Result EnsureAllRequiredParametersAreSet(IHasChildren node, Result result) {
        var missingParameters = node.Parameters.Where(p => p is { IsRequired: true, IsSet: false }).Select(p => p.Name).ToArray();
        return missingParameters.Length > 0
                   ? Invalid($"Required parameter is missing: '{string.Join("', '", missingParameters)}'.")
                   : result;
    }
}
