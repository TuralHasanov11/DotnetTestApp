namespace WebApp.Domain.Shared;

public static class ResultExtensions
{
    public static Result<T> Ensure<T>(this Result<T> result, Predicate<T> predicate, Error error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return predicate(result.Value) ? result : Result.Failure<T>(error);
    }

    public static Result<TOutput> Map<TInput, TOutput>(this Result<TInput> result, Func<TInput, TOutput> mappingFunc)
    {
        return result.IsSuccess ?
            Result.Success(mappingFunc(result.Value)) :
            Result.Failure<TOutput>(result.Error);
    }

    public static async Task<Result> Bind<TInput>(this Result<TInput> result, Func<TInput, Task<Result>> func)
    {
        if(result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        return await func(result.Value);
    }

    public static async Task<Result<TOutput>> Bind<TInput, TOutput>(this Result<TInput> result, Func<TInput, Task<Result<TOutput>>> func)
    {
        if (result.IsFailure)
        {
            return Result.Failure<TOutput>(result.Error);
        }

        return await func(result.Value);
    }
}