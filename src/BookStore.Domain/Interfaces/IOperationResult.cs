namespace BookStore.Domain.Interfaces
{
    public interface IOperationResult<out T> : IOperationResult
    {
        T? Payload { get; }
    }

    public interface IOperationResult
    {
        bool Success { get; }
        string? Message { get; }
        Exception? Exception { get; }
    }
}