namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var result = await categoryService.GetAll(ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var categoryResultDtoList = result.Payload!.ToDto();
            return Ok(categoryResultDtoList);
        }

        [HttpGet("pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllWithPagination(
            [Range(1, int.MaxValue)] int pageNumber = 1, 
            [Range(1, 100)] int pageSize = 10, 
            CancellationToken ct = default)
        {
            var result = await categoryService.GetAllWithPagination(pageNumber, pageSize, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var categoriesResultDto = result.Payload!.ToDto(c => c.ToDto());
            return Ok(categoriesResultDto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var result = await categoryService.GetById(id, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var categoryResultDto = result.Payload!.ToDto();
            return Ok(categoryResultDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(CategoryAddDto categoryDto, CancellationToken ct = default)
        {
            var category = categoryDto.ToModel();
            var categoryResult = await categoryService.Add(category, ct);

            if (!categoryResult.Success)
            {
                return categoryResult.ToActionResult();
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
                return BadRequest(new ErrorResponse("ID mismatch"));
            }

            var category = categoryDto.ToModel();
            var categoryResult = await categoryService.Update(category, ct);

            if (!categoryResult.Success)
            {
                return categoryResult.ToActionResult();
            }

            var categoryResultDto = categoryResult.Payload!.ToDto();
            return Ok(categoryResultDto);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(int id, CancellationToken ct = default)
        {
            var result = await categoryService.Remove(id, ct);
            return result.ToActionResult();
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct = default)
        {
            var result = await categoryService.Search(q, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var categoriesResultDto = result.Payload!.ToDto();
            return Ok(categoriesResultDto);
        }
    }
}
