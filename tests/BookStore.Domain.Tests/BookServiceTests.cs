using System.Linq.Expressions;
using AutoFixture;
using BookStore.Domain.Constants;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Domain.Services;
using BookStore.Domain.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace BookStore.Domain.Tests
{
    public class BookServiceTests
    {
        private readonly Fixture _fixture;
        private readonly IBookRepository _bookRepositoryMock;
        private readonly ICategoryRepository _categoryRepositoryMock;
        private readonly ILogger<BookService> _loggerMock;
        private readonly BookService _service;

        public BookServiceTests()
        {
            _fixture = FixtureFactory.Create();
            _bookRepositoryMock = Substitute.For<IBookRepository>();
            _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
            _loggerMock = Substitute.For<ILogger<BookService>>();
            _service = new BookService(_bookRepositoryMock, _categoryRepositoryMock, _loggerMock);
        }

        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_BookRepositoryIsNull()
        {
            // Arrange & Act
            Action act = () => new BookService(null!, _categoryRepositoryMock, _loggerMock);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("bookRepository");
        }

        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_CategoryRepositoryIsNull()
        {
            // Arrange & Act
            Action act = () => new BookService(_bookRepositoryMock, null!, _loggerMock);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("categoryRepository");
        }

        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_LoggerIsNull()
        {
            // Arrange & Act
            Action act = () => new BookService(_bookRepositoryMock, _categoryRepositoryMock, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [Fact]
        public async Task GetAll_Should_ReturnListOfBooks_When_BooksExist()
        {
            // Arrange
            var books = _fixture.CreateMany<Book>(3).ToList();
            _bookRepositoryMock.GetAll(Arg.Any<CancellationToken>()).Returns(books);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_Should_CallRepositoryOnce_When_Called()
        {
            // Arrange
            _bookRepositoryMock.GetAll(Arg.Any<CancellationToken>()).Returns(new List<Book>());

            // Act
            await _service.GetAll();

            // Assert
            await _bookRepositoryMock.Received(1).GetAll(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllWithPagination_Should_ReturnPagedResponse_When_BooksExist()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var books = _fixture.CreateMany<Book>(5).ToList();
            var pagedResponse = new PagedResponse<Book>(books, pageNumber, pageSize, books.Count);
            _bookRepositoryMock.GetAllWithPagination(pageNumber, pageSize, Arg.Any<CancellationToken>()).Returns(pagedResponse);

            // Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Data.Should().HaveCount(5);
            result.Payload.PageNumber.Should().Be(pageNumber);
            result.Payload.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task GetAllWithPagination_Should_CallRepositoryOnce_When_Called()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var pagedResponse = _fixture.Create<PagedResponse<Book>>();
            _bookRepositoryMock.GetAllWithPagination(pageNumber, pageSize, Arg.Any<CancellationToken>()).Returns(pagedResponse);

            // Act
            await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            await _bookRepositoryMock.Received(1).GetAllWithPagination(pageNumber, pageSize, Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        [InlineData(-100, 10)]
        public async Task GetAllWithPagination_Should_ReturnValidationError_When_PageNumberIsInvalid(int pageNumber, int pageSize)
        {
            // Arrange & Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Contain("Page number");
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(1, -10)]
        public async Task GetAllWithPagination_Should_ReturnValidationError_When_PageSizeIsTooSmall(int pageNumber, int pageSize)
        {
            // Arrange & Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Contain("Page size");
        }

        [Theory]
        [InlineData(1, 101)]
        [InlineData(1, 200)]
        [InlineData(1, 1000)]
        public async Task GetAllWithPagination_Should_ReturnValidationError_When_PageSizeIsTooLarge(int pageNumber, int pageSize)
        {
            // Arrange & Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Contain("Page size");
        }

        [Fact]
        public async Task GetAllWithPagination_Should_NotCallRepository_When_ValidationFails()
        {
            // Arrange & Act
            await _service.GetAllWithPagination(0, 10);

            // Assert
            await _bookRepositoryMock.DidNotReceive().GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetById_Should_ReturnBook_When_BookExists()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock.GetById(book.Id, Arg.Any<CancellationToken>()).Returns(book);

            // Act
            var result = await _service.GetById(book.Id);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload!.Id.Should().Be(book.Id);
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound_When_BookDoesNotExist()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            _bookRepositoryMock.GetById(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

            // Act
            var result = await _service.GetById(bookId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        }

        [Fact]
        public async Task GetById_Should_CallRepositoryOnce_When_Called()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            var book = _fixture.Create<Book>();
            _bookRepositoryMock.GetById(bookId, Arg.Any<CancellationToken>()).Returns(book);

            // Act
            await _service.GetById(bookId);

            // Assert
            await _bookRepositoryMock.Received(1).GetById(bookId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetBooksByCategory_Should_ReturnListOfBooks_When_BooksExistForCategory()
        {
            // Arrange
            var categoryId = 1;
            var books = _fixture.Build<Book>()
                .With(b => b.CategoryId, categoryId)
                .CreateMany(3)
                .ToList();
            _bookRepositoryMock.GetBooksByCategory(categoryId, Arg.Any<CancellationToken>()).Returns(books);

            // Act
            var result = await _service.GetBooksByCategory(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload.Should().HaveCount(3);
            result.Payload.All(b => b.CategoryId == categoryId).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetBooksByCategory_Should_ReturnValidationError_When_CategoryIdIsInvalid(int invalidCategoryId)
        {
            // Arrange & Act
            var result = await _service.GetBooksByCategory(invalidCategoryId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
        }

        [Fact]
        public async Task GetBooksByCategory_Should_CallRepositoryOnce_When_CategoryIdIsValid()
        {
            // Arrange
            var categoryId = 1;
            var books = _fixture.CreateMany<Book>(2).ToList();
            _bookRepositoryMock.GetBooksByCategory(categoryId, Arg.Any<CancellationToken>()).Returns(books);

            // Act
            await _service.GetBooksByCategory(categoryId);

            // Assert
            await _bookRepositoryMock.Received(1).GetBooksByCategory(categoryId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetBooksByCategory_Should_NotCallRepository_When_CategoryIdIsInvalid()
        {
            // Arrange & Act
            await _service.GetBooksByCategory(0);

            // Assert
            await _bookRepositoryMock.DidNotReceive().GetBooksByCategory(Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Search_Should_ReturnBooks_When_MatchingBooksExist()
        {
            // Arrange
            var searchTerm = "Clean Code";
            var books = _fixture.Build<Book>()
                .With(b => b.Name, searchTerm)
                .CreateMany(2)
                .ToList();
            _bookRepositoryMock.Search(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(books);

            // Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_Should_ReturnValidationError_When_SearchTermIsNull()
        {
            // Arrange & Act
            var result = await _service.Search(null!);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Search_Should_ReturnValidationError_When_SearchTermIsWhitespace(string searchTerm)
        {
            // Arrange & Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
        }

        [Fact]
        public async Task Search_Should_CallRepositorySearch_When_SearchTermIsValid()
        {
            // Arrange
            var searchTerm = "Clean Code";
            var books = _fixture.CreateMany<Book>(2).ToList();
            _bookRepositoryMock.Search(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(books);

            // Act
            await _service.Search(searchTerm);

            // Assert
            await _bookRepositoryMock.Received(1).Search(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Search_Should_NotCallRepository_When_SearchTermIsInvalid()
        {
            // Arrange & Act
            await _service.Search("   ");

            // Assert
            await _bookRepositoryMock.DidNotReceive().Search(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task SearchBookWithCategory_Should_ReturnBooks_When_MatchingBooksExist()
        {
            // Arrange
            var searchTerm = "Programming";
            var books = _fixture.CreateMany<Book>(3).ToList();
            _bookRepositoryMock.SearchBookWithCategory(searchTerm, Arg.Any<CancellationToken>()).Returns(books);

            // Act
            var result = await _service.SearchBookWithCategory(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload.Should().HaveCount(3);
        }

        [Fact]
        public async Task SearchBookWithCategory_Should_ReturnValidationError_When_SearchTermIsNull()
        {
            // Arrange & Act
            var result = await _service.SearchBookWithCategory(null!);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchBookWithCategory_Should_ReturnValidationError_When_SearchTermIsWhitespace(string searchTerm)
        {
            // Arrange & Act
            var result = await _service.SearchBookWithCategory(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
        }

        [Fact]
        public async Task SearchBookWithCategory_Should_CallRepositorySearch_When_SearchTermIsValid()
        {
            // Arrange
            var searchTerm = "Programming";
            var books = _fixture.CreateMany<Book>().ToList();
            _bookRepositoryMock.SearchBookWithCategory(searchTerm, Arg.Any<CancellationToken>()).Returns(books);

            // Act
            await _service.SearchBookWithCategory(searchTerm);

            // Assert
            await _bookRepositoryMock.Received(1).SearchBookWithCategory(searchTerm, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task SearchBookWithCategory_Should_NotCallRepository_When_SearchTermIsInvalid()
        {
            // Arrange & Act
            await _service.SearchBookWithCategory("   ");

            // Assert
            await _bookRepositoryMock.DidNotReceive().SearchBookWithCategory(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Add_Should_AddBook_When_BookIsValid()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Name, "Clean Code")
                .With(b => b.CategoryId, 1)
                .Create();
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.Add(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().Be(book);
            result.ErrorCode.Should().Be(OperationErrorCode.None);
        }

        [Fact]
        public async Task Add_Should_ReturnValidationError_When_BookIsNull()
        {
            // Arrange & Act
            var result = await _service.Add(null!);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.EntityCannotBeNull, "Book"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Add_Should_ReturnValidationError_When_BookNameIsInvalid(string? invalidName)
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Name, invalidName!)
                .Create();

            // Act
            var result = await _service.Add(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.FieldRequired, "Book name"));
        }

        [Fact]
        public async Task Add_Should_ReturnNotFound_When_CategoryDoesNotExist()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Name, "Valid Book Name")
                .With(b => b.CategoryId, 999)
                .Create();
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.Add(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Message.Should().Be(string.Format(ErrorMessages.CategoryNotFound, book.CategoryId));
        }

        [Fact]
        public async Task Add_Should_ReturnDuplicateError_When_BookNameAlreadyExists()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Name, "Existing Book")
                .Create();
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _service.Add(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.Duplicate);
            result.Message.Should().Be(ErrorMessages.BookDuplicate);
        }

        [Fact]
        public async Task Add_Should_CallRepositoryAdd_When_BookIsValid()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Name, "Valid Book")
                .Create();
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            await _service.Add(book);

            // Assert
            await _bookRepositoryMock.Received(1).Add(book, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Add_Should_ThrowException_When_RepositoryThrowsException()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Name, "Valid Book")
                .Create();
            var exceptionMessage = "Database error";
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);
            _bookRepositoryMock.Add(book, Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception(exceptionMessage)));

            // Act
            Func<Task> act = async () => await _service.Add(book);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(exceptionMessage);
        }

        [Fact]
        public async Task Update_Should_UpdateBook_When_BookIsValid()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, 1)
                .With(b => b.Name, "Updated Book")
                .Create();
            _bookRepositoryMock.GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>()).Returns(book);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().Be(book);
            result.ErrorCode.Should().Be(OperationErrorCode.None);
        }

        [Fact]
        public async Task Update_Should_ReturnValidationError_When_BookIsNull()
        {
            // Arrange & Act
            var result = await _service.Update(null!);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.EntityCannotBeNull, "Book"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Update_Should_ReturnValidationError_When_BookNameIsInvalid(string? invalidName)
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, 1)
                .With(b => b.Name, invalidName!)
                .Create();

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.FieldRequired, "Book name"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task Update_Should_ReturnValidationError_When_BookIdIsInvalid(int invalidId)
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, invalidId)
                .With(b => b.Name, "Valid Name")
                .Create();

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.InvalidId, "book"));
        }

        [Fact]
        public async Task Update_Should_ReturnNotFound_When_BookDoesNotExist()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, 1)
                .With(b => b.Name, "Valid Book")
                .Create();
            _bookRepositoryMock.GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>()).Returns((Book?)null);

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Message.Should().Be(string.Format(ErrorMessages.BookNotFound, book.Id));
        }

        [Fact]
        public async Task Update_Should_ReturnNotFound_When_CategoryDoesNotExist()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, 1)
                .With(b => b.Name, "Valid Book")
                .With(b => b.CategoryId, 999)
                .Create();
            _bookRepositoryMock.GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>()).Returns(book);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Message.Should().Be(string.Format(ErrorMessages.CategoryNotFound, book.CategoryId));
        }

        [Fact]
        public async Task Update_Should_ReturnDuplicateError_When_BookNameExistsForDifferentBook()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, 1)
                .With(b => b.Name, "Duplicate Name")
                .Create();
            _bookRepositoryMock.GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>()).Returns(book);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.Duplicate);
            result.Message.Should().Be(ErrorMessages.BookDuplicateOnUpdate);
        }

        [Fact]
        public async Task Update_Should_CallRepositoryUpdate_When_BookIsValid()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, 1)
                .With(b => b.Name, "Valid Book")
                .Create();
            _bookRepositoryMock.GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>()).Returns(book);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            await _service.Update(book);

            // Assert
            await _bookRepositoryMock.Received(1).Update(book, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Update_Should_ThrowException_When_RepositoryThrowsException()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(b => b.Id, 1)
                .With(b => b.Name, "Valid Book")
                .Create();
            var exceptionMessage = "Database error";
            _bookRepositoryMock.GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>()).Returns(book);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);
            _bookRepositoryMock.Update(book, Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception(exceptionMessage)));

            // Act
            Func<Task> act = async () => await _service.Update(book);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(exceptionMessage);
        }

        [Fact]
        public async Task Remove_Should_RemoveBook_When_BookExists()
        {
            // Arrange
            var bookId = 1;
            var book = _fixture.Build<Book>()
                .With(b => b.Id, bookId)
                .Create();
            _bookRepositoryMock.GetById(bookId, Arg.Any<CancellationToken>()).Returns(book);

            // Act
            var result = await _service.Remove(bookId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().BeTrue();
            result.ErrorCode.Should().Be(OperationErrorCode.None);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task Remove_Should_ReturnValidationError_When_BookIdIsInvalid(int invalidId)
        {
            // Arrange & Act
            var result = await _service.Remove(invalidId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.InvalidId, "book"));
        }

        [Fact]
        public async Task Remove_Should_ReturnNotFound_When_BookDoesNotExist()
        {
            // Arrange
            var bookId = 1;
            _bookRepositoryMock.GetById(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

            // Act
            var result = await _service.Remove(bookId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Message.Should().Be(string.Format(ErrorMessages.BookNotFound, bookId));
        }

        [Fact]
        public async Task Remove_Should_CallRepositoryRemove_When_BookCanBeRemoved()
        {
            // Arrange
            var bookId = 1;
            var book = _fixture.Build<Book>()
                .With(b => b.Id, bookId)
                .Create();
            _bookRepositoryMock.GetById(bookId, Arg.Any<CancellationToken>()).Returns(book);

            // Act
            await _service.Remove(bookId);

            // Assert
            await _bookRepositoryMock.Received(1).Remove(book, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Remove_Should_ThrowException_When_RepositoryThrowsException()
        {
            // Arrange
            var bookId = 1;
            var book = _fixture.Build<Book>()
                .With(b => b.Id, bookId)
                .Create();
            var exceptionMessage = "Database error";
            _bookRepositoryMock.GetById(bookId, Arg.Any<CancellationToken>()).Returns(book);
            _bookRepositoryMock.Remove(book, Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception(exceptionMessage)));

            // Act
            Func<Task> act = async () => await _service.Remove(bookId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(exceptionMessage);
        }
    }
}
