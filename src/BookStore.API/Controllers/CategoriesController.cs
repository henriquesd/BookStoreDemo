using BookStore.API.Dtos.Category;
using BookStore.API.Mappings;
using BookStore.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var categories = await _categoryService.GetAll(ct);

            var categoryResultDtoList = categories.ToDto();

            return Ok(categoryResultDtoList);
        }

        [HttpGet("GetAllWithPagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllWithPagination(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page number and page size must be greater than zero" });
            }

            var paginatedCategories = await _categoryService.GetAllWithPagination(pageNumber, pageSize, ct);

            var categoriesResultDto = paginatedCategories.ToDto(c => c.ToDto());

            return Ok(categoriesResultDto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var category = await _categoryService.GetById(id, ct);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            var categoryResultDto = category.ToDto();

            return Ok(categoryResultDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(CategoryAddDto categoryDto, CancellationToken ct = default)
        {
            var category = categoryDto.ToModel();
            var categoryResult = await _categoryService.Add(category, ct);

            if (!categoryResult.Success)
            {
                return BadRequest(new { message = categoryResult.Message });
            }

            var categoryResultDto = categoryResult.Payload!.ToDto();

            return CreatedAtAction(nameof(GetById), new { id = categoryResultDto.Id }, categoryResultDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, CategoryEditDto categoryDto, CancellationToken ct = default)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var category = categoryDto.ToModel();
            var categoryResult = await _categoryService.Update(category, ct);

            return categoryResult.ToActionResult(c => c.ToDto());
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(int id, CancellationToken ct = default)
        {
            var result = await _categoryService.Remove(id, ct);

            return result.ToActionResult();
        }

        [HttpGet]
        [Route("search/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search(string category, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var categories = await _categoryService.Search(category, ct);

            if (!categories.Any())
            {
                return NotFound(new { message = "No categories were found" });
            }

            return Ok(categories.ToDto());
        }
    }
}
