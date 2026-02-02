using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.Domain.Services
{
    public static class ValidationHelper
    {
        public static OperationResult<T> ValidateNotNull<T>(T? entity, string entityName) where T : class
        {
            if (entity == null)
            {
                return OperationResult<T>.ValidationError($"{entityName} cannot be null");
            }

            return new OperationResult<T>(true, null);
        }

        public static OperationResult<T> ValidateRequiredString<T>(string? value, string fieldName) where T : class
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return OperationResult<T>.ValidationError($"{fieldName} is required");
            }

            return new OperationResult<T>(true, null);
        }

        public static OperationResult<T> ValidateId<T>(int id, string entityName) where T : class
        {
            if (id <= 0)
            {
                return OperationResult<T>.ValidationError($"Invalid {entityName} ID");
            }

            return new OperationResult<T>(true, null);
        }

        public static OperationResult<bool> ValidateIdForRemoval(int id, string entityName)
        {
            if (id <= 0)
            {
                return OperationResult<bool>.ValidationError($"Invalid {entityName} ID");
            }

            return new OperationResult<bool>(true, null);
        }
    }
}
