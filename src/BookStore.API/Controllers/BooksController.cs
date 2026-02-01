using AutoMapper;
using BookStore.API.Dtos;
using BookStore.API.Dtos.Book;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : MainController
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(IMapper mapper,
                                IBookService bookService)
        {
            _mapper = mapper;
            _bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAll();

            return Ok(_mapper.Map<IEnumerable<BookResultDto>>(books));
        }

        [HttpGet("GetAllWithPagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWithPagination(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }

            var paginatedBooks = await _bookService.GetAllWithPagination(pageNumber, pageSize);

            var booksResultDto = _mapper.Map<PagedResponseDto<BookResultDto>>(paginatedBooks);

            return Ok(booksResultDto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetById(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookResultDto>(book));
        }

        [HttpGet]
        [Route("get-books-by-category/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBooksByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new { message = "Invalid category ID" });
            }

            var books = await _bookService.GetBooksByCategory(categoryId);

            if (!books.Any())
            {
                return NotFound(new { message = "No books found for this category" });
            }

            return Ok(_mapper.Map<IEnumerable<BookResultDto>>(books));
        }

        [HttpPost]
        [ProducesResponseType(typeof(OperationResult<BookResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult<BookResultDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(BookAddDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var book = _mapper.Map<Book>(bookDto);
            var bookResult = await _bookService.Add(book);

            var result = _mapper.Map<OperationResult<BookResultDto>>(bookResult);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(OperationResult<BookResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult<BookResultDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, BookEditDto bookDto)
        {
            if (id != bookDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var book = _mapper.Map<Book>(bookDto);
            var bookResult = await _bookService.Update(book);

            var result = _mapper.Map<OperationResult<BookResultDto>>(bookResult);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _bookService.Remove(id);

            if (!result.Success)
            {
                return result.Message.Contains("not found")
                    ? NotFound(new { message = result.Message })
                    : BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        [HttpGet]
        [Route("search/{bookName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search(string bookName)
        {
            if (string.IsNullOrWhiteSpace(bookName))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var books = await _bookService.Search(bookName);

            if (!books.Any())
            {
                return NotFound(new { message = "No books were found" });
            }

            return Ok(_mapper.Map<IEnumerable<BookResultDto>>(books));
        }

        [HttpGet]
        [Route("search-book-with-category/{searchedValue}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchBookWithCategory(string searchedValue)
        {
            if (string.IsNullOrWhiteSpace(searchedValue))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var books = await _bookService.SearchBookWithCategory(searchedValue);

            if (!books.Any())
            {
                return NotFound(new { message = "No books were found" });
            }

            return Ok(_mapper.Map<IEnumerable<BookResultDto>>(books));
        }
    }
}