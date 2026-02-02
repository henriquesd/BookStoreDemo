using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Models
{
    public record OperationResult<T> : IOperationResult<T>
    {
        public T? Payload { get; init; }
        public bool Success { get; init; }
        public string? Message { get; init; }
        public OperationErrorCode ErrorCode { get; init; }

        public OperationResult(T? payload)
            : this(payload, true, null, OperationErrorCode.None)
        {
        }

        public OperationResult(bool success, string? message, OperationErrorCode errorCode = OperationErrorCode.ValidationError)
            : this(default, success, message, success ? OperationErrorCode.None : errorCode)
        {
        }

        public OperationResult(T? payload, bool success, string? message, OperationErrorCode errorCode = OperationErrorCode.None)
        {
            Payload = payload;
            Success = success;
            Message = message;
            ErrorCode = errorCode;
        }

        public static OperationResult<T> NotFound(string message) =>
            new(default, false, message, OperationErrorCode.NotFound);

        public static OperationResult<T> Duplicate(string message) =>
            new(default, false, message, OperationErrorCode.Duplicate);

        public static OperationResult<T> ValidationError(string message) =>
            new(default, false, message, OperationErrorCode.ValidationError);

        public static OperationResult<T> HasDependencies(string message) =>
            new(default, false, message, OperationErrorCode.HasDependencies);

        public static OperationResult<T> Error(string message) =>
            new(default, false, message, OperationErrorCode.UnexpectedError);

        public static OperationResult<T> Ok(T payload) =>
            new(payload);
    }
}