using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Models
{
    public record OperationResult<T> : IOperationResult<T>
    {
        public T? Payload { get; init; }
        public bool Success { get; init; }
        public string? Message { get; init; }

        public OperationResult(T? payload)
            : this(payload, true, null)
        {
        }

        public OperationResult(bool success, string? message)
            : this(default, success, message)
        {
        }

        public OperationResult(T? payload, bool success, string? message)
        {
            Payload = payload;
            Success = success;
            Message = message;
        }
    }
}