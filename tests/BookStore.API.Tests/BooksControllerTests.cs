using AutoFixture;
using BookStore.API.Controllers;
using BookStore.API.Dtos;
using BookStore.API.Dtos.Book;
using BookStore.API.Mappings;
using BookStore.API.Tests.Helpers;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookStore.API.Tests
{
    public class BooksControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _fixture = FixtureFactory.Create();
            _bookServiceMock = new Mock<IBookService>();
            _controller = new BooksController(_bookServiceMock.Object);
        }

        private Book CreateBook(int id = 1, string name = "Test Book", int categoryId = 1)
        {
            return new Book
            {
                Id = id,
                Name = name,
                Author = "Test Author",
                Description = "Test Description",
                Value = 29.99m,
                CategoryId = categoryId,
                PublishDate = DateTime.Now,
                Category = new Category { Id = categoryId, Name = "Test Category" }
            };
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithBooks_WhenBooksExist()
        {
            var books = new List<Book>
            {
                CreateBook(1, "Book 1"),
                CreateBook(2, "Book 2"),
                CreateBook(3, "Book 3")
            };
            _bookServiceMock.Setup(s => s.GetAll()).ReturnsAsync(books);

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedBooks = okResult.Value as IEnumerable<BookResultDto>;
            returnedBooks.Should().HaveCount(3);
            returnedBooks.First().Name.Should().Be("Book 1");
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoBooksExist()
        {
            _bookServiceMock.Setup(s => s.GetAll()).ReturnsAsync(new List<Book>());

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedBooks = okResult.Value as IEnumerable<BookResultDto>;
            returnedBooks.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ShouldCallServiceOnce_WhenCalled()
        {
            _bookServiceMock.Setup(s => s.GetAll()).ReturnsAsync(new List<Book>());

            await _controller.GetAll();

            _bookServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        [Theory]
        [InlineData(1, 10, 3)]
        [InlineData(2, 5, 10)]
        [InlineData(1, 20, 1)]
        public async Task GetAllWithPagination_ShouldReturnOkWithPagedBooks_WhenBooksExist(int pageNumber, int pageSize, int totalRecords)
        {
            var books = Enumerable.Range(1, totalRecords)
                .Select(i => CreateBook(i, $"Book {i}"))
                .ToList();
            var pagedResponse = new PagedResponse<Book>(books, pageNumber, pageSize, totalRecords);
            _bookServiceMock.Setup(s => s.GetAllWithPagination(pageNumber, pageSize)).ReturnsAsync(pagedResponse);

            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var pagedDto = (PagedResponseDto<BookResultDto>)okResult.Value;
            pagedDto.Data.Should().HaveCount(totalRecords);
            pagedDto.PageNumber.Should().Be(pageNumber);
            pagedDto.PageSize.Should().Be(pageSize);
            pagedDto.TotalRecords.Should().Be(totalRecords);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnOkWithEmptyList_WhenNoBooksExist()
        {
            var pagedResponse = new PagedResponse<Book>(new List<Book>(), 1, 10, 0);
            _bookServiceMock.Setup(s => s.GetAllWithPagination(1, 10)).ReturnsAsync(pagedResponse);

            var result = await _controller.GetAllWithPagination();

            result.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(-5, -5)]
        public async Task GetAllWithPagination_ShouldReturnBadRequest_WhenParametersAreInvalid(int pageNumber, int pageSize)
        {
            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallServiceOnce_WhenCalled()
        {
            var pagedResponse = new PagedResponse<Book>(new List<Book>(), 1, 10, 0);
            _bookServiceMock.Setup(s => s.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

            await _controller.GetAllWithPagination(1, 10);

            _bookServiceMock.Verify(s => s.GetAllWithPagination(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithBook_WhenBookExists()
        {
            var book = CreateBook(1, "Test Book");
            _bookServiceMock.Setup(s => s.GetById(1)).ReturnsAsync(book);

            var result = await _controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult.Value as BookResultDto;
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Test Book");
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            _bookServiceMock.Setup(s => s.GetById(It.IsAny<int>())).ReturnsAsync((Book)null);

            var result = await _controller.GetById(999);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetById_ShouldCallServiceOnce_WhenCalled(int bookId)
        {
            var book = CreateBook(bookId);
            _bookServiceMock.Setup(s => s.GetById(bookId)).ReturnsAsync(book);

            await _controller.GetById(bookId);

            _bookServiceMock.Verify(s => s.GetById(bookId), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldReturnCreatedWithBook_WhenBookIsValid()
        {
            var dto = _fixture.Create<BookAddDto>();
            var book = dto.ToModel();
            book.Id = 1; // Set ID to simulate saved entity
            var operationResult = new OperationResult<Book>(book, true, null);
            _bookServiceMock.Setup(s => s.Add(It.IsAny<Book>())).ReturnsAsync(operationResult);

            var result = await _controller.Add(dto);

            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var resultDto = createdResult.Value as BookResultDto;
            resultDto.Should().NotBeNull();
            resultDto.Id.Should().Be(1);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenServiceReturnsFails()
        {
            var dto = _fixture.Create<BookAddDto>();
            var operationResult = new OperationResult<Book>(false, "Book already exists");
            _bookServiceMock.Setup(s => s.Add(It.IsAny<Book>())).ReturnsAsync(operationResult);

            var result = await _controller.Add(dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Add_ShouldCallServiceOnce_WhenCalled()
        {
            var dto = _fixture.Create<BookAddDto>();
            var book = dto.ToModel();
            var operationResult = new OperationResult<Book>(book, true, null);
            _bookServiceMock.Setup(s => s.Add(It.IsAny<Book>())).ReturnsAsync(operationResult);

            await _controller.Add(dto);

            _bookServiceMock.Verify(s => s.Add(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnOkWithBook_WhenBookIsValid()
        {
            var dto = _fixture.Create<BookEditDto>();
            var book = dto.ToModel();
            var operationResult = new OperationResult<Book>(book, true, null);
            _bookServiceMock.Setup(s => s.Update(It.IsAny<Book>())).ReturnsAsync(operationResult);

            var result = await _controller.Update(dto.Id, dto);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultDto = okResult.Value as BookResultDto;
            resultDto.Should().NotBeNull();
            resultDto.Id.Should().Be(dto.Id);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 10)]
        [InlineData(100, 999)]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch(int urlId, int dtoId)
        {
            var dto = _fixture.Build<BookEditDto>().With(b => b.Id, dtoId).Create();

            var result = await _controller.Update(urlId, dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenServiceReturnsNotFound()
        {
            var dto = _fixture.Create<BookEditDto>();
            var operationResult = OperationResult<Book>.NotFound("Book not found");
            _bookServiceMock.Setup(s => s.Update(It.IsAny<Book>())).ReturnsAsync(operationResult);

            var result = await _controller.Update(dto.Id, dto);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldCallServiceOnce_WhenCalled()
        {
            var dto = _fixture.Create<BookEditDto>();
            var book = dto.ToModel();
            var operationResult = new OperationResult<Book>(book, true, null);
            _bookServiceMock.Setup(s => s.Update(It.IsAny<Book>())).ReturnsAsync(operationResult);

            await _controller.Update(dto.Id, dto);

            _bookServiceMock.Verify(s => s.Update(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task Remove_ShouldReturnNoContent_WhenBookIsRemoved()
        {
            var operationResult = new OperationResult<bool>(true, true, null);

            _bookServiceMock
                .Setup(s => s.Remove(1))
                .ReturnsAsync(operationResult);

            var result = await _controller.Remove(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            var operationResult = OperationResult<bool>.NotFound("Book with ID 1 not found");

            _bookServiceMock
                .Setup(s => s.Remove(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);

            var result = await _controller.Remove(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnBadRequest_WhenServiceFails()
        {
            var operationResult = new OperationResult<bool>(false, "Cannot delete book");

            _bookServiceMock
                .Setup(s => s.Remove(1))
                .ReturnsAsync(operationResult);

            var result = await _controller.Remove(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(999)]
        public async Task Remove_ShouldCallServiceOnce_WhenCalled(int bookId)
        {
            var operationResult = new OperationResult<bool>(true, true, null);

            _bookServiceMock
                .Setup(s => s.Remove(bookId))
                .ReturnsAsync(operationResult);

            await _controller.Remove(bookId);

            _bookServiceMock.Verify(s => s.Remove(bookId), Times.Once);
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldReturnOkWithBooks_WhenBooksExist()
        {
            var books = new List<Book> { CreateBook(1), CreateBook(2) };
            _bookServiceMock.Setup(s => s.GetBooksByCategory(1)).ReturnsAsync(books);

            var result = await _controller.GetBooksByCategory(1);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult.Value as IEnumerable<BookResultDto>;
            dtos.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldReturnNotFound_WhenNoBooksExist()
        {
            _bookServiceMock.Setup(s => s.GetBooksByCategory(1)).ReturnsAsync(new List<Book>());

            var result = await _controller.GetBooksByCategory(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task GetBooksByCategory_ShouldReturnBadRequest_WhenCategoryIdIsInvalid(int categoryId)
        {
            var result = await _controller.GetBooksByCategory(categoryId);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData("Test")]
        [InlineData("Book")]
        [InlineData("Author")]
        public async Task Search_ShouldReturnOkWithBooks_WhenBooksFound(string searchTerm)
        {
            var books = new List<Book> { CreateBook(1, searchTerm) };
            _bookServiceMock.Setup(s => s.Search(searchTerm)).ReturnsAsync(books);

            var result = await _controller.Search(searchTerm);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult.Value as IEnumerable<BookResultDto>;
            dtos.Should().HaveCount(1);
        }

        [Fact]
        public async Task Search_ShouldReturnNotFound_WhenNoBooksFound()
        {
            _bookServiceMock.Setup(s => s.Search("NonExistent")).ReturnsAsync(new List<Book>());

            var result = await _controller.Search("NonExistent");

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Search_ShouldReturnBadRequest_WhenSearchTermIsEmpty(string searchTerm)
        {
            var result = await _controller.Search(searchTerm);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnOkWithBooks_WhenBooksFound()
        {
            var books = new List<Book> { CreateBook(1) };
            _bookServiceMock.Setup(s => s.SearchBookWithCategory("Test")).ReturnsAsync(books);

            var result = await _controller.SearchBookWithCategory("Test");

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult.Value as IEnumerable<BookResultDto>;
            dtos.Should().HaveCount(1);
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnNotFound_WhenNoBooksFound()
        {
            _bookServiceMock.Setup(s => s.SearchBookWithCategory("NonExistent")).ReturnsAsync(new List<Book>());

            var result = await _controller.SearchBookWithCategory("NonExistent");

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task SearchBookWithCategory_ShouldReturnBadRequest_WhenSearchTermIsEmpty(string searchTerm)
        {
            var result = await _controller.SearchBookWithCategory(searchTerm);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
