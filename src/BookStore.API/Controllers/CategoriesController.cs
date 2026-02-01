using BookStore.API.Dtos;
using BookStore.API.Dtos.Category;
using BookStore.API.Mappings;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : MainController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAll();

            var categoryResultDtoList = categories.ToDto();

            return Ok(categoryResultDtoList);
        }

        [HttpGet("GetAllWithPagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllWithPagination(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page number and page size must be greater than zero" });
            }

            var paginatedCategories = await _categoryService.GetAllWithPagination(pageNumber, pageSize);

            var categoriesResultDto = paginatedCategories.ToDto(c => c.ToDto());

            return Ok(categoriesResultDto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetById(id);

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
        public async Task<IActionResult> Add(CategoryAddDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = categoryDto.ToModel();
            var categoryResult = await _categoryService.Add(category);

            if (!categoryResult.Success)
            {
                return BadRequest(new { message = categoryResult.Message });
            }

            var categoryResultDto = categoryResult.Payload.ToDto();

            return CreatedAtAction(nameof(GetById), new { id = categoryResultDto.Id }, categoryResultDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, CategoryEditDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = categoryDto.ToModel();
            var categoryResult = await _categoryService.Update(category);

            if (!categoryResult.Success)
            {
                return categoryResult.Message.Contains("not found")
                    ? NotFound(new { message = categoryResult.Message })
                    : BadRequest(new { message = categoryResult.Message });
            }

            var categoryResultDto = categoryResult.Payload.ToDto();

            return Ok(categoryResultDto);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _categoryService.Remove(id);

            if (!result.Success)
            {
                return result.Message.Contains("not found")
                    ? NotFound(new { message = result.Message })
                    : BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        [HttpGet]
        [Route("search/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var categories = await _categoryService.Search(category);

            if (!categories.Any())
            {
                return NotFound(new { message = "No categories were found" });
            }

            return Ok(categories.ToDto());
        }
    }
}