using BookStore.API.Dtos.Category;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.API.Mappings
{
    public static class CategoryMappingExtensions
    {
        public static Category ToModel(this CategoryAddDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new Category
            {
                Name = dto.Name
            };
        }

        public static Category ToModel(this CategoryEditDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new Category
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }

        public static CategoryResultDto ToDto(this Category model)
        {
            if (model == null)
            {
                return null;
            }

            return new CategoryResultDto
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public static IEnumerable<CategoryResultDto> ToDto(this IEnumerable<Category> models)
        {
            if (models == null)
            {
                return null;
            }

            return models.Select(m => m.ToDto()).ToList();
        }

        public static OperationResult<CategoryResultDto> ToDto(this IOperationResult<Category> operationResult)
        {
            if (operationResult == null)
            {
                return null;
            }

            return new OperationResult<CategoryResultDto>(
                operationResult.Payload?.ToDto(),
                operationResult.Success,
                operationResult.Message
            );
        }
    }
}
