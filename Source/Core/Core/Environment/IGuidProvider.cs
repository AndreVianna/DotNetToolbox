﻿namespace DotNetToolbox.Environment;

public interface IGuidProvider {
    Guid New();
    Guid New(byte[] bytes);
    Guid New(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k);
    Guid New(int a, short b, short c, byte[] d);
    Guid New(ReadOnlySpan<byte> bytes);
    Guid New(ReadOnlySpan<byte> bytes, bool bigEndian);
    Guid New(string text);
    Guid New(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k);
    Guid Parse(ReadOnlySpan<char> input);
    Guid Parse(ReadOnlySpan<char> s, IFormatProvider? provider);
    Guid Parse(string input);
    Guid Parse(string s, IFormatProvider? provider);
    Guid ParseExact(ReadOnlySpan<char> input, [Syntax("GuidFormat")] ReadOnlySpan<char> format);
    Guid ParseExact(string input, [Syntax("GuidFormat")] string format);
    bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Guid result);
    bool TryParse(ReadOnlySpan<char> input, out Guid result);
    bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Guid result);
    bool TryParse([NotNullWhen(true)] string? input, out Guid result);
    bool TryParseExact(ReadOnlySpan<char> input, [Syntax("GuidFormat")] ReadOnlySpan<char> format, out Guid result);
    bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true), Syntax("GuidFormat")] string? format, out Guid result);
}
