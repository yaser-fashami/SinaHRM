using SinaHRM.Domain.Exceptions;

namespace SinaHRM.Domain.Common;

public record Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public string? Message { get; }

    protected Result(bool isSuccess, Error? error, string? message)
    {
        IsSuccess = isSuccess;
        Error = error?? Error.None;
        Message = message;
    }

    public static Result Success(string? message = default)
    {
        return new Result(true, Error.None, message);
    }

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (string.IsNullOrWhiteSpace(error.Code))
        {
            throw new ArgumentException(
                "Error code cannot be empty.",
                nameof(error));
        }

        return new Result(false, error, null);
    }

}

public sealed record Result<T> : Result
{
    private T? _value;

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "Cannot access the value of a failed result.");


    private Result(bool isSuccess, T? value, Error? error, string? message):base(isSuccess, error, message)
    {
        _value = value;
    }

    public static Result<T> Success(T value, string? message = default)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<T>(true, value, Error.None, message);
    }

    public static new Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (string.IsNullOrWhiteSpace(error.Code))
        {
            throw new ArgumentException(
                "Error code cannot be empty.",
                nameof(error));
        }

        return new Result<T>(false, default, error, null);
    }
}
