
namespace SinaHRM.Utilities;

public sealed record Error(
    string Code,
    string Message,
    Exception? Exception = null)
{
    public static readonly Error None = new("", "");
}


public sealed class Result<T>
{
    private T? _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "Cannot access the value of a failed result.");

    public Error Error { get; }

    private Result(bool isSuccess, T? value, Error error)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<T>(true, value, Error.None);
    }

    public static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (string.IsNullOrWhiteSpace(error.Code))
        {
            throw new ArgumentException(
                "Error code cannot be empty.",
                nameof(error));
        }

        return new Result<T>(false, default, error);
    }
}
