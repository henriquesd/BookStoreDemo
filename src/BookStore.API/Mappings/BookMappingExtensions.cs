using BookStore.API.Dtos.Book;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.API.Mappings
{
    public static class BookMappingExtensions
    {
        public static Book ToModel(this BookAddDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new Book
            {
                Name = dto.Name,
                Author = dto.Author,
                Description = dto.Description,
                Value = dto.Value,
                PublishDate = dto.PublishDate,
                CategoryId = dto.CategoryId
            };
        }

        public static Book ToModel(this BookEditDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new Book
            {
                Id = dto.Id,
                Name = dto.Name,
                Author = dto.Author,
                Description = dto.Description,
                Value = dto.Value,
                PublishDate = dto.PublishDate,
                CategoryId = dto.CategoryId
            };
        }

        public static BookResultDto ToDto(this Book model)
        {
            if (model == null)
            {
                return null;
            }

            return new BookResultDto
            {
                Id = model.Id,
                Name = model.Name,
                Author = model.Author,
                Description = model.Description,
                Value = model.Value,
                PublishDate = model.PublishDate,
                CategoryId = model.CategoryId,
                CategoryName = model.Category?.Name
            };
        }

        public static IEnumerable<BookResultDto> ToDto(this IEnumerable<Book> models)
        {
            if (models == null)
            {
                return null;
            }

            return models.Select(m => m.ToDto()).ToList();
        }

        public static OperationResult<BookResultDto> ToDto(this IOperationResult<Book> operationResult)
        {
            if (operationResult == null)
            {
                return null;
            }

            return new OperationResult<BookResultDto>(
                operationResult.Payload?.ToDto(),
                operationResult.Success,
                operationResult.Message
            );
        }
    }
}
