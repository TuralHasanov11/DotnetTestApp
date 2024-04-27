namespace WebApp.Domain.Shared;

public interface IValidationResult
{
    public static readonly Error ValidationError = Error.Validation("ValidationError", "A validation error");

    Error[] Errors { get; }
}
