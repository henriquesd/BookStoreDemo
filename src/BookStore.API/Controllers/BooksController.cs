using BookStore.API.Dtos.Book;
using BookStore.API.Mappings;
using BookStore.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var books = await _bookService.GetAll(ct);

            return Ok(books.ToDto());
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

            var paginatedBooks = await _bookService.GetAllWithPagination(pageNumber, pageSize, ct);

            var booksResultDto = paginatedBooks.ToDto(b => b.ToDto());

            return Ok(booksResultDto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var book = await _bookService.GetById(id, ct);

            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            return Ok(book.ToDto());
        }

        [HttpGet]
        [Route("get-books-by-category/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBooksByCategory(int categoryId, CancellationToken ct = default)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new { message = "Invalid category ID" });
            }

            var books = await _bookService.GetBooksByCategory(categoryId, ct);

            if (!books.Any())
            {
                return NotFound(new { message = "No books found for this category" });
            }

            return Ok(books.ToDto());
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(BookAddDto bookDto, CancellationToken ct = default)
        {
            var book = bookDto.ToModel();
            var bookResult = await _bookService.Add(book, ct);

            if (!bookResult.Success)
            {
                return BadRequest(new { message = bookResult.Message });
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
                return BadRequest(new { message = "ID mismatch" });
            }

            var book = bookDto.ToModel();
            var bookResult = await _bookService.Update(book, ct);

            if (!bookResult.Success)
            {
                return bookResult.ErrorCode == OperationErrorCode.NotFound
                    ? NotFound(new { message = bookResult.Message })
                    : BadRequest(new { message = bookResult.Message });
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
            var result = await _bookService.Remove(id, ct);

            if (!result.Success)
            {
                return result.ErrorCode == OperationErrorCode.NotFound
                    ? NotFound(new { message = result.Message })
                    : BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        [HttpGet]
        [Route("search/{bookName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search(string bookName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(bookName))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var books = await _bookService.Search(bookName, ct);

            if (!books.Any())
            {
                return NotFound(new { message = "No books were found" });
            }

            return Ok(books.ToDto());
        }

        [HttpGet]
        [Route("search-book-with-category/{searchedValue}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchBookWithCategory(string searchedValue, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(searchedValue))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var books = await _bookService.SearchBookWithCategory(searchedValue, ct);

            if (!books.Any())
            {
                return NotFound(new { message = "No books were found" });
            }

            return Ok(books.ToDto());
        }
    }
}
