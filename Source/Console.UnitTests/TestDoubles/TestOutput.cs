﻿namespace DotNetToolbox.ConsoleApplication.TestDoubles;

internal class TestOutput() : IOutput {
    public List<string> Lines { get; } = [ string.Empty ];

    public void ClearScreen() {
        Lines.Clear();
        Lines.Add(string.Empty);
    }

    public void ResetColor() => throw new NotImplementedException();

    public void Write(bool value) => throw new NotImplementedException();

    public void Write(char value) => throw new NotImplementedException();

    public void Write(char[] buffer, int index, int count) => throw new NotImplementedException();

    public void Write(char[]? buffer) => throw new NotImplementedException();

    public void Write(decimal value) => throw new NotImplementedException();

    public void Write(double value) => throw new NotImplementedException();

    public void Write(float value) => throw new NotImplementedException();

    public void Write(int value) => throw new NotImplementedException();

    public void Write(long value) => throw new NotImplementedException();

    public void Write(object? value) => throw new NotImplementedException();

    public void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) => throw new NotImplementedException();

    public void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) => throw new NotImplementedException();

    public void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) => throw new NotImplementedException();

    public void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        => Lines[^1] += string.Format(format, args);

    public void Write(string? value) => Lines[^1] += value ?? string.Empty;

    public void Write(StringBuilder? builder) => throw new NotImplementedException();

    public void Write(uint value) => throw new NotImplementedException();

    public void Write(ulong value) => throw new NotImplementedException();

    public void WriteLine() => Lines.Add(string.Empty);

    public void WriteLine(bool value) => throw new NotImplementedException();

    public void WriteLine(char value) => throw new NotImplementedException();

    public void WriteLine(char[] buffer, int index, int count) => throw new NotImplementedException();

    public void WriteLine(char[]? buffer) => throw new NotImplementedException();

    public void WriteLine(decimal value) => throw new NotImplementedException();

    public void WriteLine(double value) => throw new NotImplementedException();

    public void WriteLine(float value) => throw new NotImplementedException();

    public void WriteLine(int value) => throw new NotImplementedException();

    public void WriteLine(long value) => throw new NotImplementedException();

    public void WriteLine(object? value) => throw new NotImplementedException();

    public void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) => throw new NotImplementedException();

    public void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) => throw new NotImplementedException();

    public void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) => throw new NotImplementedException();

    public void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args) {
        Write(format, args);
        WriteLine();
    }

    public void WriteLine(string? value) => Lines.Add(value ?? string.Empty);

    public void WriteLine(StringBuilder? builder) => throw new NotImplementedException();

    public void WriteLine(uint value) => throw new NotImplementedException();

    public void WriteLine(ulong value) => throw new NotImplementedException();

    public ConsoleColor BackgroundColor {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Encoding Encoding {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    public TextWriter Error => throw new NotImplementedException();
    public ConsoleColor ForegroundColor {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public TextWriter Writer => throw new NotImplementedException();
}
