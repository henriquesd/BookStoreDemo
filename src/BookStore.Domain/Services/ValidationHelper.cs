using BookStore.Domain.Constants;
using BookStore.Domain.Models;

namespace BookStore.Domain.Services
{
    public static class ValidationHelper
    {
        public static OperationResult<T> ValidateNotNull<T>(T? entity, string entityName) where T : class
        {
            if (entity == null)
            {
                return OperationResult<T>.ValidationError(string.Format(ErrorMessages.EntityCannotBeNull, entityName));
            }

            return OperationResult<T>.SuccessResult();
        }

        public static OperationResult<T> ValidateRequiredString<T>(string? value, string fieldName) where T : class
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return OperationResult<T>.ValidationError(string.Format(ErrorMessages.FieldRequired, fieldName));
            }

            return OperationResult<T>.SuccessResult();
        }

        public static OperationResult<T> ValidateId<T>(int id, string entityName) where T : class
        {
            if (id <= 0)
            {
                return OperationResult<T>.ValidationError(string.Format(ErrorMessages.InvalidId, entityName));
            }

            return OperationResult<T>.SuccessResult();
        }

        public static OperationResult<bool> ValidateIdForRemoval(int id, string entityName)
        {
            if (id <= 0)
            {
                return OperationResult<bool>.ValidationError(string.Format(ErrorMessages.InvalidId, entityName));
            }

            return OperationResult<bool>.SuccessResult();
        }

        public static OperationResult<PagedResponse<T>> ValidatePagination<T>(int pageNumber, int pageSize) where T : class
        {
            if (pageNumber <= 0)
            {
                return OperationResult<PagedResponse<T>>.ValidationError("Page number must be greater than zero");
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                return OperationResult<PagedResponse<T>>.ValidationError("Page size must be between 1 and 100");
            }

            return OperationResult<PagedResponse<T>>.SuccessResult();
        }
    }
}
