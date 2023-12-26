using BookStore.API.Dtos.Category;
using BookStore.Domain.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace BookStore.API.Controllers.SwaggerExamples.Categories
{
    [ExcludeFromCodeCoverage]
    public class AddCategoryResponseExample : IMultipleExamplesProvider<OperationResult<CategoryResultDto>>
    {
        public IEnumerable<SwaggerExample<OperationResult<CategoryResultDto>>> GetExamples()
        {
            var payload = new CategoryResultDto
            {
                Id = 1,
                Name = "Category name"
            };

            yield return SwaggerExample.Create(ExampleConstants.SuccessfulResponse, new OperationResult<CategoryResultDto>(payload));

            yield return SwaggerExample.Create(ExampleConstants.FailedResponse, new OperationResult<CategoryResultDto>(payload, false, "This category name is already being used"));
        }
    }
}