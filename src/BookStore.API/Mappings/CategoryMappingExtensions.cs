namespace BookStore.API.Mappings
{
    public static class CategoryMappingExtensions
    {
        public static Category ToModel(this CategoryAddDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new Category
            {
                Name = dto.Name
            };
        }

        public static Category ToModel(this CategoryEditDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new Category
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }

        public static CategoryResultDto ToDto(this Category model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return new CategoryResultDto
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public static IEnumerable<CategoryResultDto> ToDto(this IEnumerable<Category> models)
        {
            ArgumentNullException.ThrowIfNull(models);

            return models.Select(m => m.ToDto()).ToList();
        }

        public static OperationResult<CategoryResultDto> ToDto(this IOperationResult<Category> operationResult)
        {
            ArgumentNullException.ThrowIfNull(operationResult);

            if (operationResult.Success && operationResult.Payload != null)
            {
                return OperationResult<CategoryResultDto>.SuccessResult(operationResult.Payload.ToDto());
            }

            // Map error results
            return operationResult.ErrorCode switch
            {
                OperationErrorCode.NotFound => OperationResult<CategoryResultDto>.NotFound(operationResult.Message ?? "Not found"),
                OperationErrorCode.Duplicate => OperationResult<CategoryResultDto>.Duplicate(operationResult.Message ?? "Duplicate"),
                OperationErrorCode.HasDependencies => OperationResult<CategoryResultDto>.HasDependencies(operationResult.Message ?? "Has dependencies"),
                OperationErrorCode.ValidationError => OperationResult<CategoryResultDto>.ValidationError(operationResult.Message ?? "Validation error"),
                _ => OperationResult<CategoryResultDto>.Failure(operationResult.Message ?? "An error occurred")
            };
        }
    }
}
