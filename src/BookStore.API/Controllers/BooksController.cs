namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var result = await bookService.GetAll(ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var booksResultDto = result.Payload!.ToDto();
            return Ok(booksResultDto);
        }

        [HttpGet("pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllWithPagination(
            [Range(1, int.MaxValue)] int pageNumber = 1,
            [Range(1, 100)] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await bookService.GetAllWithPagination(pageNumber, pageSize, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var booksResultDto = result.Payload!.ToDto(b => b.ToDto());
            return Ok(booksResultDto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var result = await bookService.GetById(id, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var bookResultDto = result.Payload!.ToDto();
            return Ok(bookResultDto);
        }

        [HttpGet("categories/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBooksByCategory(int categoryId, CancellationToken ct = default)
        {
            var result = await bookService.GetBooksByCategory(categoryId, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var booksResultDto = result.Payload!.ToDto();
            return Ok(booksResultDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(BookAddDto bookDto, CancellationToken ct = default)
        {
            var book = bookDto.ToModel();
            var bookResult = await bookService.Add(book, ct);

            if (!bookResult.Success)
            {
                return bookResult.ToActionResult();
            }

            var bookResultDto = bookResult.Payload!.ToDto();
            return CreatedAtAction(nameof(GetById), new { id = bookResultDto.Id }, bookResultDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BookResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, BookEditDto bookDto, CancellationToken ct = default)
        {
            if (id != bookDto.Id)
            {
                return BadRequest(new ErrorResponse("ID mismatch"));
            }

            var book = bookDto.ToModel();
            var bookResult = await bookService.Update(book, ct);

            if (!bookResult.Success)
            {
                return bookResult.ToActionResult();
            }

            var bookResultDto = bookResult.Payload!.ToDto();
            return Ok(bookResultDto);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(int id, CancellationToken ct = default)
        {
            var result = await bookService.Remove(id, ct);
            return result.ToActionResult();
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct = default)
        {
            var result = await bookService.Search(q, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var booksResultDto = result.Payload!.ToDto();
            return Ok(booksResultDto);
        }

        [HttpGet("search-with-category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchBookWithCategory([FromQuery] string q, CancellationToken ct = default)
        {
            var result = await bookService.SearchBookWithCategory(q, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var booksResultDto = result.Payload!.ToDto();
            return Ok(booksResultDto);
        }

        [HttpGet("search/pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchWithPagination(
            [FromQuery] string q,
            [Range(1, int.MaxValue)] int pageNumber = 1,
            [Range(1, 100)] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await bookService.SearchWithPagination(q, pageNumber, pageSize, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var booksResultDto = result.Payload!.ToDto(b => b.ToDto());
            return Ok(booksResultDto);
        }

        [HttpGet("search-with-category/pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchBookWithCategoryPagination(
            [FromQuery] string q,
            [Range(1, int.MaxValue)] int pageNumber = 1,
            [Range(1, 100)] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await bookService.SearchBookWithCategoryPagination(q, pageNumber, pageSize, ct);

            if (!result.Success)
            {
                return result.ToActionResult();
            }

            var booksResultDto = result.Payload!.ToDto(b => b.ToDto());
            return Ok(booksResultDto);
        }
    }
}
