using BookStore.API.Dtos.Category;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace BookStore.API.Controllers.SwaggerExamples.Categories
{
    [ExcludeFromCodeCoverage]
    public class AddCategoryRequestExample : IExamplesProvider<CategoryAddDto>
    {
        public CategoryAddDto GetExamples()
        {
            return new CategoryAddDto
            {
                Name = "Category name"
            };
        }
    }
}