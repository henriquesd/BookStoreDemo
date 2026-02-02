using BookStore.API.Dtos.Book;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.API.Mappings
{
    public static class BookMappingExtensions
    {
        public static Book ToModel(this BookAddDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

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
            ArgumentNullException.ThrowIfNull(dto);

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
            ArgumentNullException.ThrowIfNull(model);

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
            ArgumentNullException.ThrowIfNull(models);

            return models.Select(m => m.ToDto()).ToList();
        }

        public static OperationResult<BookResultDto> ToDto(this IOperationResult<Book> operationResult)
        {
            ArgumentNullException.ThrowIfNull(operationResult);

            return new OperationResult<BookResultDto>(
                operationResult.Payload?.ToDto(),
                operationResult.Success,
                operationResult.Message
            );
        }
    }
}
