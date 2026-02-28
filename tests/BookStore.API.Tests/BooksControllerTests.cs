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
using NSubstitute;
using Xunit;

namespace BookStore.API.Tests
{
    public class BooksControllerTests
    {
        private readonly Fixture _fixture;
        private readonly IBookService _bookServiceMock;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _fixture = FixtureFactory.Create();
            _bookServiceMock = Substitute.For<IBookService>();
            _controller = new BooksController(_bookServiceMock);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithBooks_WhenBooksExist()
        {
            // Arrange
            var books = _fixture.CreateMany<Book>(3).ToList();
            _bookServiceMock.GetAll().Returns(OperationResult<IEnumerable<Book>>.Ok(books));

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedBooks = okResult!.Value as IEnumerable<BookResultDto>;
            returnedBooks.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoBooksExist()
        {
            // Arrange
            _bookServiceMock.GetAll().Returns(OperationResult<IEnumerable<Book>>.Ok(new List<Book>()));

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedBooks = okResult!.Value as IEnumerable<BookResultDto>;
            returnedBooks.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            _bookServiceMock.GetAll().Returns(OperationResult<IEnumerable<Book>>.Ok(new List<Book>()));

            // Act
            await _controller.GetAll();

            // Assert
            await _bookServiceMock.Received(1).GetAll();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnOkWithPagedBooks_WhenBooksExist()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var books = _fixture.CreateMany<Book>(5).ToList();
            var pagedResponse = new PagedResponse<Book>(books, pageNumber, pageSize, books.Count);
            _bookServiceMock.GetAllWithPagination(pageNumber, pageSize).Returns(OperationResult<PagedResponse<Book>>.Ok(pagedResponse));

            // Act
            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var pagedDto = (PagedResponseDto<BookResultDto>)okResult!.Value!;
            pagedDto.Data.Should().HaveCount(books.Count);
            pagedDto.PageNumber.Should().Be(pageNumber);
            pagedDto.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnOkWithEmptyList_WhenNoBooksExist()
        {
            // Arrange
            var pagedResponse = new PagedResponse<Book>(new List<Book>(), 1, 10, 0);
            _bookServiceMock.GetAllWithPagination(1, 10).Returns(OperationResult<PagedResponse<Book>>.Ok(pagedResponse));

            // Act
            var result = await _controller.GetAllWithPagination();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(-5, -5)]
        [InlineData(1, 101)] // pageSize > 100
        public async Task GetAllWithPagination_ShouldReturnBadRequest_WhenParametersAreInvalid(int pageNumber, int pageSize)
        {
            // Arrange
            var validationError = OperationResult<PagedResponse<Book>>.ValidationError("Page number and page size must be valid");
            _bookServiceMock.GetAllWithPagination(pageNumber, pageSize).Returns(validationError);

            // Act
            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var pagedResponse = new PagedResponse<Book>(new List<Book>(), pageNumber, pageSize, 0);
            _bookServiceMock.GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>()).Returns(OperationResult<PagedResponse<Book>>.Ok(pagedResponse));

            // Act
            await _controller.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            await _bookServiceMock.Received(1).GetAllWithPagination(pageNumber, pageSize);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithBook_WhenBookExists()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookServiceMock.GetById(book.Id).Returns(OperationResult<Book>.Ok(book));

            // Act
            var result = await _controller.GetById(book.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult!.Value as BookResultDto;
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(book.Id);
            dto.Name.Should().Be(book.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            _bookServiceMock.GetById(Arg.Any<int>()).Returns(OperationResult<Book>.NotFound($"Book with ID {bookId} not found"));

            // Act
            var result = await _controller.GetById(bookId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookServiceMock.GetById(book.Id).Returns(OperationResult<Book>.Ok(book));

            // Act
            await _controller.GetById(book.Id);

            // Assert
            await _bookServiceMock.Received(1).GetById(book.Id);
        }

        [Fact]
        public async Task Add_ShouldReturnCreatedWithBook_WhenBookIsValid()
        {
            // Arrange
            var dto = _fixture.Create<BookAddDto>();
            var bookFromDto = dto.ToModel();
            var book = new Book
            {
                Id = _fixture.Create<int>(),
                Name = bookFromDto.Name,
                Author = bookFromDto.Author,
                Description = bookFromDto.Description,
                Value = bookFromDto.Value,
                CategoryId = bookFromDto.CategoryId,
                PublishDate = bookFromDto.PublishDate
            };
            var operationResult = OperationResult<Book>.SuccessResult(book);
            _bookServiceMock.Add(Arg.Any<Book>()).Returns(operationResult);

            // Act
            var result = await _controller.Add(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var resultDto = createdResult!.Value as BookResultDto;
            resultDto.Should().NotBeNull();
            resultDto!.Id.Should().Be(book.Id);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenServiceReturnsFails()
        {
            // Arrange
            var dto = _fixture.Create<BookAddDto>();
            var errorMessage = _fixture.Create<string>();
            var operationResult = OperationResult<Book>.ValidationError(errorMessage);
            _bookServiceMock.Add(Arg.Any<Book>()).Returns(operationResult);

            // Act
            var result = await _controller.Add(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Add_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var dto = _fixture.Create<BookAddDto>();
            var book = dto.ToModel();
            var operationResult = OperationResult<Book>.SuccessResult(book);
            _bookServiceMock.Add(Arg.Any<Book>()).Returns(operationResult);

            // Act
            await _controller.Add(dto);

            // Assert
            await _bookServiceMock.Received(1).Add(Arg.Any<Book>());
        }

        [Fact]
        public async Task Update_ShouldReturnOkWithBook_WhenBookIsValid()
        {
            // Arrange
            var dto = _fixture.Create<BookEditDto>();
            var book = dto.ToModel();
            var operationResult = OperationResult<Book>.SuccessResult(book);
            _bookServiceMock.Update(Arg.Any<Book>()).Returns(operationResult);

            // Act
            var result = await _controller.Update(dto.Id, dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultDto = okResult!.Value as BookResultDto;
            resultDto.Should().NotBeNull();
            resultDto!.Id.Should().Be(dto.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var dto = _fixture.Create<BookEditDto>();
            var differentUrlId = dto.Id + _fixture.Create<int>() + 1;

            // Act
            var result = await _controller.Update(differentUrlId, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenServiceReturnsNotFound()
        {
            // Arrange
            var dto = _fixture.Create<BookEditDto>();
            var operationResult = OperationResult<Book>.NotFound(_fixture.Create<string>());
            _bookServiceMock.Update(Arg.Any<Book>()).Returns(operationResult);

            // Act
            var result = await _controller.Update(dto.Id, dto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var dto = _fixture.Create<BookEditDto>();
            var book = dto.ToModel();
            var operationResult = OperationResult<Book>.SuccessResult(book);
            _bookServiceMock.Update(Arg.Any<Book>()).Returns(operationResult);

            // Act
            await _controller.Update(dto.Id, dto);

            // Assert
            await _bookServiceMock.Received(1).Update(Arg.Any<Book>());
        }

        [Fact]
        public async Task Remove_ShouldReturnNoContent_WhenBookIsRemoved()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.SuccessResult(true);
            _bookServiceMock.Remove(bookId).Returns(operationResult);

            // Act
            var result = await _controller.Remove(bookId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.NotFound(_fixture.Create<string>());
            _bookServiceMock.Remove(bookId, Arg.Any<CancellationToken>()).Returns(operationResult);

            // Act
            var result = await _controller.Remove(bookId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.ValidationError(_fixture.Create<string>());
            _bookServiceMock.Remove(bookId).Returns(operationResult);

            // Act
            var result = await _controller.Remove(bookId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Remove_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.SuccessResult(true);
            _bookServiceMock.Remove(bookId).Returns(operationResult);

            // Act
            await _controller.Remove(bookId);

            // Assert
            await _bookServiceMock.Received(1).Remove(bookId);
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldReturnOkWithBooks_WhenBooksExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var books = _fixture
                .Build<Book>()
                .With(b => b.CategoryId, categoryId)
                .CreateMany(3)
                .ToList();
            _bookServiceMock.GetBooksByCategory(categoryId).Returns(OperationResult<IEnumerable<Book>>.Ok(books));

            // Act
            var result = await _controller.GetBooksByCategory(categoryId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<BookResultDto>;
            dtos.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldReturnOkWithEmptyList_WhenNoBooksExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            _bookServiceMock.GetBooksByCategory(categoryId).Returns(OperationResult<IEnumerable<Book>>.Ok(new List<Book>()));

            // Act
            var result = await _controller.GetBooksByCategory(categoryId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<BookResultDto>;
            dtos.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task GetBooksByCategory_ShouldReturnBadRequest_WhenCategoryIdIsInvalid(int categoryId)
        {
            // Arrange
            var validationError = OperationResult<IEnumerable<Book>>.ValidationError("Invalid category ID");
            _bookServiceMock.GetBooksByCategory(categoryId).Returns(validationError);

            // Act
            var result = await _controller.GetBooksByCategory(categoryId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Search_ShouldReturnOkWithBooks_WhenBooksFound()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var books = _fixture.CreateMany<Book>(2).ToList();
            _bookServiceMock.Search(searchTerm).Returns(OperationResult<IEnumerable<Book>>.Ok(books));

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<BookResultDto>;
            dtos.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_ShouldReturnOkWithEmptyList_WhenNoBooksFound()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            _bookServiceMock.Search(searchTerm).Returns(OperationResult<IEnumerable<Book>>.Ok(new List<Book>()));

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<BookResultDto>;
            dtos.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Search_ShouldReturnBadRequest_WhenSearchTermIsEmpty(string? searchTerm)
        {
            // Arrange
            var validationError = OperationResult<IEnumerable<Book>>.ValidationError("Search term is required");
            _bookServiceMock.Search(searchTerm!).Returns(validationError);

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnOkWithBooks_WhenBooksFound()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var books = _fixture.CreateMany<Book>(2).ToList();
            _bookServiceMock.SearchBookWithCategory(searchTerm).Returns(OperationResult<IEnumerable<Book>>.Ok(books));

            // Act
            var result = await _controller.SearchBookWithCategory(searchTerm);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<BookResultDto>;
            dtos.Should().HaveCount(2);
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnOkWithEmptyList_WhenNoBooksFound()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            _bookServiceMock.SearchBookWithCategory(searchTerm).Returns(OperationResult<IEnumerable<Book>>.Ok(new List<Book>()));

            // Act
            var result = await _controller.SearchBookWithCategory(searchTerm);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<BookResultDto>;
            dtos.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task SearchBookWithCategory_ShouldReturnBadRequest_WhenSearchTermIsEmpty(string? searchTerm)
        {
            // Arrange
            var validationError = OperationResult<IEnumerable<Book>>.ValidationError("Search term is required");
            _bookServiceMock.SearchBookWithCategory(searchTerm!).Returns(validationError);

            // Act
            var result = await _controller.SearchBookWithCategory(searchTerm);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
