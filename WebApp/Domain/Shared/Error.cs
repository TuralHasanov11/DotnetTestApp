namespace WebApp.Domain.Shared;

public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Value is null", ErrorType.Failure);

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    public Error(string code, string message, ErrorType errorType)
    {
        Code = code;
        Message = message;
        Type = errorType;
    }

    public static implicit operator string (Error error) => error.Code;

    public static Error NotFound(string code, string message) => new Error(code, message, ErrorType.NotFound);
    public static Error Validation(string code, string message) => new Error(code, message, ErrorType.Validation);
    public static Error Conflict(string code, string message) => new Error(code, message, ErrorType.Conflict);
    public static Error Failure(string code, string message) => new Error(code, message, ErrorType.Failure);
}


public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3
}