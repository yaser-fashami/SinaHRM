namespace SinaHRM.Domain.Exceptions;

public record Error
{
    public Error(string? code, string message, Exception? exception = null)
    {
        Code = code;
        Message = message;
        Exception = exception;
    }

    public string? Code { get; }
    public string Message { get; }
    public Exception? Exception { get; }

    public static Error None => new Error(null, "");
}


public sealed record ExceptionError : Error
{
    public ExceptionError(Exception ex) : base(ex.HResult.ToString(), ex.Message, ex)
    {
    }
}

public sealed record EmptyListError : Error
{
    public EmptyListError(): base("301", "this collection is empty")
    {
    }
}

public sealed record InProcessContractExistError : Error
{
    public InProcessContractExistError(string period, string employeeName): base("201", $"قرار داد {employeeName} در دوره {period} موجود و در جریان است.")
    {
    }
}

public sealed record ContractTemplateVersionDoesnotExistError : Error
{
    public ContractTemplateVersionDoesnotExistError(): base("202", "ContractTemplateVersion Doesn`t Exist!")
    {
    }
}

public sealed record CountOperationError : Error
{
    public CountOperationError(): base("302", "Enumeration encountered a problem!")
    {
    }
}

