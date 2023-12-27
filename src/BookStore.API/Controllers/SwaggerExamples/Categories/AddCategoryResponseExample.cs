using BookStore.API.Dtos.Category;
using BookStore.Domain.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace BookStore.API.Controllers.SwaggerExamples.Categories
{
    [ExcludeFromCodeCoverage]
    public class AddCategoryResponseExample : IExamplesProvider<OperationResult<CategoryResultDto>>
    {
        public OperationResult<CategoryResultDto> GetExamples()
        {
            var payload = new CategoryResultDto
            {
                Id = 1,
                Name = "Category name"
            };

            return new OperationResult<CategoryResultDto>(payload);
        }
    }
}