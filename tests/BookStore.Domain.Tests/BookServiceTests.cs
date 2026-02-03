using System.Linq.Expressions;
using AutoFixture;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Domain.Services;
using BookStore.Domain.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BookStore.Domain.Tests
{
    public class BookServiceTests
    {
        private readonly Fixture _fixture;
        private readonly IBookRepository _bookRepositoryMock;
        private readonly ICategoryRepository _categoryRepositoryMock;
        private readonly BookService _service;

        public BookServiceTests()
        {
            _fixture = FixtureFactory.Create();
            _bookRepositoryMock = Substitute.For<IBookRepository>();
            _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
            _service = new BookService(_bookRepositoryMock, _categoryRepositoryMock);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfBooks_WhenBooksExist()
        {
            // Arrange
            var books = _fixture.CreateMany<Book>(3).ToList();
            _bookRepositoryMock.GetAll().Returns(books);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNull_WhenBooksDoNotExist()
        {
            // Arrange
            _bookRepositoryMock.GetAll().Returns((List<Book>?)null);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            _bookRepositoryMock.GetAll().Returns(new List<Book>());

            // Act
            await _service.GetAll();

            // Assert
            await _bookRepositoryMock.Received(1).GetAll();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnPagedResponse_WhenBooksExist()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var books = _fixture.CreateMany<Book>(5).ToList();
            var pagedResponse = new PagedResponse<Book>(books, pageNumber, pageSize, books.Count);
            _bookRepositoryMock.GetAllWithPagination(pageNumber, pageSize).Returns(pagedResponse);

            // Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PagedResponse<Book>>();
            result.Data.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnEmptyList_WhenBooksDoNotExist()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var pagedResponse = new PagedResponse<Book>(new List<Book>(), pageNumber, pageSize, 0);
            _bookRepositoryMock.GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>()).Returns(pagedResponse);

            // Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var pagedResponse = _fixture.Create<PagedResponse<Book>>();
            _bookRepositoryMock.GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>()).Returns(pagedResponse);

            // Act
            await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            await _bookRepositoryMock.Received(1).GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>());
        }

        [Fact]
        public async Task GetById_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock.GetById(book.Id).Returns(book);

            // Act
            var result = await _service.GetById(book.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Book>();
            result!.Id.Should().Be(book.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            _bookRepositoryMock.GetById(Arg.Any<int>()).Returns((Book?)null);

            // Act
            var result = await _service.GetById(bookId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock.GetById(book.Id).Returns(book);

            // Act
            await _service.GetById(book.Id);

            // Assert
            await _bookRepositoryMock.Received(1).GetById(book.Id);
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldReturnListOfBooks_WhenBooksExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var books = _fixture.Build<Book>()
                .With(b => b.CategoryId, categoryId)
                .CreateMany(3)
                .ToList();
            _bookRepositoryMock.GetBooksByCategory(categoryId).Returns(books);

            // Act
            var result = await _service.GetBooksByCategory(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(3);
            result!.All(b => b.CategoryId == categoryId).Should().BeTrue();
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldReturnNull_WhenBooksDoNotExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            _bookRepositoryMock.GetBooksByCategory(Arg.Any<int>()).Returns((IEnumerable<Book>?)null);

            // Act
            var result = await _service.GetBooksByCategory(categoryId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var books = _fixture.CreateMany<Book>(2).ToList();
            _bookRepositoryMock.GetBooksByCategory(categoryId).Returns(books);

            // Act
            await _service.GetBooksByCategory(categoryId);

            // Assert
            await _bookRepositoryMock.Received(1).GetBooksByCategory(categoryId);
        }

        [Fact]
        public async Task Search_ShouldReturnListOfBooks_WhenBooksWithSearchedNameExist()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var books = _fixture.Build<Book>()
                .With(b => b.Name, searchTerm)
                .CreateMany(2)
                .ToList();
            _bookRepositoryMock.Search(Arg.Any<Expression<Func<Book, bool>>>()).Returns(books);

            // Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_ShouldReturnNull_WhenBooksWithSearchedNameDoNotExist()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            _bookRepositoryMock.Search(Arg.Any<Expression<Func<Book, bool>>>()).Returns((IEnumerable<Book>?)null);

            // Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Search_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var books = _fixture.CreateMany<Book>().ToList();
            _bookRepositoryMock.Search(Arg.Any<Expression<Func<Book, bool>>>()).Returns(books);

            // Act
            await _service.Search(searchTerm);

            // Assert
            await _bookRepositoryMock.Received(1).Search(Arg.Any<Expression<Func<Book, bool>>>());
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnListOfBooks_WhenBooksExist()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var books = _fixture.CreateMany<Book>(3).ToList();
            _bookRepositoryMock.SearchBookWithCategory(searchTerm).Returns(books);

            // Act
            var result = await _service.SearchBookWithCategory(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnNull_WhenBooksDoNotExist()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            _bookRepositoryMock.SearchBookWithCategory(Arg.Any<string>()).Returns((IEnumerable<Book>?)null);

            // Act
            var result = await _service.SearchBookWithCategory(searchTerm);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var books = _fixture.CreateMany<Book>().ToList();
            _bookRepositoryMock.SearchBookWithCategory(searchTerm).Returns(books);

            // Act
            await _service.SearchBookWithCategory(searchTerm);

            // Assert
            await _bookRepositoryMock.Received(1).SearchBookWithCategory(searchTerm);
        }

        [Fact]
        public async Task Add_ShouldAddBook_WhenBookNameDoesNotExist()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _service.Add(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().BeOfType<Book>();
        }

        [Fact]
        public async Task Add_ShouldNotAddBook_WhenCategoryDoesNotExist()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _service.Add(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Add_ShouldNotAddBook_WhenBookNameAlreadyExists()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _service.Add(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Add_ShouldCallRepositoryOnce_WhenBookIsValid()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            await _service.Add(book);

            // Assert
            await _bookRepositoryMock.Received(1).Add(book, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Update_ShouldUpdateBook_WhenBookNameDoesNotExist()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock.GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>()).Returns(book);
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().BeOfType<Book>();
        }

        [Fact]
        public async Task Update_ShouldNotUpdateBook_WhenCategoryDoesNotExist()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock
                .GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>())
                .Returns(book);
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Update_ShouldNotUpdateBook_WhenBookNameAlreadyExists()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock
                .GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>())
                .Returns(book);
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _service.Update(book);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Update_ShouldCallRepositoryOnce_WhenBookIsValid()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock
                .GetByIdAsNoTracking(book.Id, Arg.Any<CancellationToken>())
                .Returns(book);
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            await _service.Update(book);

            // Assert
            await _bookRepositoryMock.Received(1).Update(book, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Remove_ShouldReturnTrue_WhenBookCanBeRemoved()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock.GetById(book.Id).Returns(book);

            // Act
            var result = await _service.Remove(book.Id);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Remove_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = _fixture.Create<int>();
            _bookRepositoryMock.GetById(Arg.Any<int>()).Returns((Book?)null);

            // Act
            var result = await _service.Remove(bookId);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Remove_ShouldCallRepositoryOnce_WhenBookExists()
        {
            // Arrange
            var book = _fixture.Create<Book>();
            _bookRepositoryMock.GetById(book.Id).Returns(book);

            // Act
            await _service.Remove(book.Id);

            // Assert
            _bookRepositoryMock.Received(1).Remove(book);
        }
    }
}
