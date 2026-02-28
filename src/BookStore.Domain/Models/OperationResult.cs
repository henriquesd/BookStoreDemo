using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Models
{
    public record OperationResult<T> : IOperationResult<T>
    {
        public T? Payload { get; init; }
        public bool Success { get; init; }
        public string? Message { get; init; }
        public OperationErrorCode ErrorCode { get; init; }

        private OperationResult(T? payload, bool success, string? message, OperationErrorCode errorCode)
        {
            Payload = payload;
            Success = success;
            Message = message;
            ErrorCode = errorCode;
        }

        // Success factory methods
        public static OperationResult<T> SuccessResult(T payload) =>
            new(payload, true, "Success", OperationErrorCode.None);

        public static OperationResult<T> SuccessResult() =>
            new(default, true, "Success", OperationErrorCode.None);

        // Failure factory methods
        public static OperationResult<T> NotFound(string message) =>
            new(default, false, message, OperationErrorCode.NotFound);

        public static OperationResult<T> Duplicate(string message) =>
            new(default, false, message, OperationErrorCode.Duplicate);

        public static OperationResult<T> ValidationError(string message) =>
            new(default, false, message, OperationErrorCode.ValidationError);

        public static OperationResult<T> HasDependencies(string message) =>
            new(default, false, message, OperationErrorCode.HasDependencies);

        public static OperationResult<T> Failure(string message, OperationErrorCode errorCode = OperationErrorCode.UnexpectedError) =>
            new(default, false, message, errorCode);

        // Legacy method for backward compatibility - to be removed after migration
        [Obsolete("Use SuccessResult instead")]
        public static OperationResult<T> Ok(T payload) => SuccessResult(payload);

        // Legacy method for backward compatibility - to be removed after migration
        [Obsolete("Use Failure instead")]
        public static OperationResult<T> Error(string message) => Failure(message);
    }
}