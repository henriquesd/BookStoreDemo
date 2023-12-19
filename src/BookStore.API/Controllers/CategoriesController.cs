using AutoMapper;
using BookStore.API.Dtos.Category;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using static BookStore.API.Dtos.PaginationDto;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : MainController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(IMapper mapper,
            ICategoryService categoryService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAll();

            var categoryResultDtoList = _mapper.Map<IEnumerable<CategoryResultDto>>(categories);

            return Ok(categoryResultDtoList);
        }

        [HttpGet("GetAllWithPagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWithPagination(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber == 0 || pageSize == 0) return BadRequest();

            var paginatedCategories = await _categoryService.GetAllWithPagination(pageNumber, pageSize);

            var categoriesResultDto = _mapper.Map<PagedResponseDto<CategoryResultDto>>(paginatedCategories);

            return Ok(categoriesResultDto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetById(id);

            if (category == null) return NotFound();

            var categoryResultDto = _mapper.Map<CategoryResultDto>(category);

            return Ok(categoryResultDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(CategoryAddDto categoryDto)
        {
            if (!ModelState.IsValid) return BadRequest();

            var category = _mapper.Map<Category>(categoryDto);
            var result = await _categoryService.Add(category);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, CategoryEditDto categoryDto)
        {
            if (id != categoryDto.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();

            var category = _mapper.Map<Category>(categoryDto);
            
            var result = await _categoryService.Update(category);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remove(int id)
        {
            var category = await _categoryService.GetById(id);
            if (category == null) return NotFound();

            var result = await _categoryService.Remove(category);

            if (!result) return BadRequest();

            return Ok();
        }

        [HttpGet]
        [Route("search/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Category>>> Search(string category)
        {
            var categories = await _categoryService.Search(category);

            if (!categories.Any()) return NotFound("None category was founded");

            return Ok(categories);
        }
    }
}