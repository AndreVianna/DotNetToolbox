﻿namespace System.Validation;

public interface IValidationError {
    object?[] Arguments { get; }
    string Source { get; }
    string Message { get; }
}
