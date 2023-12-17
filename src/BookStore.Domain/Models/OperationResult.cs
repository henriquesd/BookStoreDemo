using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Models
{
    public record OperationResult<T> : IOperationResult<T>
    {
        public T? Payload { get; init; }
        public bool Success { get; init; }
        public string? Message { get; init; }
        public Exception? Exception { get; init; }

        public OperationResult(T? payload)
            : this(payload, true, null, null)
        {
        }

        public OperationResult(bool success, string? message)
            : this(default, success, null, message)
        {
        }

        public OperationResult(Exception? exception, bool success)
            : this(default, success, exception, exception?.Message)
        {
        }

        public OperationResult(T? payload, bool success, Exception? exception, string? message)
        {
            Payload = payload;
            Success = success;
            Exception = exception;
            Message = message;
        }
    }
}