namespace BookStore.Domain.Interfaces
{
    public enum OperationErrorCode
    {
        None,
        NotFound,
        Duplicate,
        ValidationError,
        HasDependencies,
        UnexpectedError
    }

    public interface IOperationResult<out T> : IOperationResult
    {
        T? Payload { get; }
    }

    public interface IOperationResult
    {
        bool Success { get; }
        string? Message { get; }
        OperationErrorCode ErrorCode { get; }
    }
}