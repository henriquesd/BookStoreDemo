using BookStore.API.Dtos.Category;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

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

            return new OperationResult<CategoryResultDto>(
                operationResult.Payload?.ToDto(),
                operationResult.Success,
                operationResult.Message
            );
        }
    }
}
