using AutoFixture;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Domain.Services;
using BookStore.Domain.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.Domain.Tests
{
    public class BookServiceTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly BookService _service;

        public BookServiceTests()
        {
            _fixture = FixtureFactory.Create();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfBooks_WhenBooksExist()
        {
            var books = _fixture
                .CreateMany<Book>(3).
                ToList();

            _bookRepositoryMock
                .Setup(r => r.GetAll())
                .ReturnsAsync(books);

            var result = await _service.GetAll();

            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNull_WhenBooksDoNotExist()
        {
            _bookRepositoryMock
                .Setup(r => r.GetAll())
                .ReturnsAsync((List<Book>)null);

            var result = await _service.GetAll();

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldCallRepositoryOnce_WhenCalled()
        {
            _bookRepositoryMock
                .Setup(r => r.GetAll())
                .ReturnsAsync(new List<Book>());

            await _service.GetAll();

            _bookRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 5)]
        [InlineData(1, 20)]
        public async Task GetAllWithPagination_ShouldReturnPagedResponse_WhenBooksExist(int pageNumber, int pageSize)
        {
            var books = _fixture
                .CreateMany<Book>(5
                ).ToList();

            var pagedResponse = new PagedResponse<Book>(books, pageNumber, pageSize, books.Count);
            _bookRepositoryMock
                .Setup(r => r.GetAllWithPagination(pageNumber, pageSize))
                .ReturnsAsync(pagedResponse);

            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<PagedResponse<Book>>();
            result.Data.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnEmptyList_WhenBooksDoNotExist()
        {
            var pagedResponse = new PagedResponse<Book>(new List<Book>(), 1, 10, 0);
            _bookRepositoryMock
                .Setup(r => r.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pagedResponse);

            var result = await _service.GetAllWithPagination(1, 10);

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallRepositoryOnce_WhenCalled()
        {
            var pagedResponse = _fixture.Create<PagedResponse<Book>>();
            _bookRepositoryMock
                .Setup(r => r.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pagedResponse);

            await _service.GetAllWithPagination(1, 10);

            _bookRepositoryMock.Verify(r => r.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetById_ShouldReturnBook_WhenBookExists(int bookId)
        {
            var book = _fixture
                .Build<Book>()
                .With(b => b.Id, bookId)
                .Create();
            
            _bookRepositoryMock
                .Setup(r => r.GetById(bookId))
                .ReturnsAsync(book);

            var result = await _service.GetById(bookId);

            result.Should().NotBeNull();
            result.Should().BeOfType<Book>();
            result.Id.Should().Be(bookId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenBookDoesNotExist()
        {
            _bookRepositoryMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync((Book)null);

            var result = await _service.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ShouldCallRepositoryOnce_WhenCalled()
        {
            _bookRepositoryMock
                .Setup(r => r.GetById(1))
                .ReturnsAsync(new Book());

            await _service.GetById(1);

            _bookRepositoryMock.Verify(r => r.GetById(1), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task GetBooksByCategory_ShouldReturnListOfBooks_WhenBooksExist(int categoryId)
        {
            var books = _fixture
                .Build<Book>()
                .With(b => b.CategoryId, categoryId)
                .CreateMany(3)
                .ToList();

            _bookRepositoryMock
                .Setup(r => r.GetBooksByCategory(categoryId))
                .ReturnsAsync(books);

            var result = await _service.GetBooksByCategory(categoryId);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(3);
            result.All(b => b.CategoryId == categoryId).Should().BeTrue();
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldReturnNull_WhenBooksDoNotExist()
        {
            _bookRepositoryMock
                .Setup(r => r.GetBooksByCategory(It.IsAny<int>()))
                .ReturnsAsync((IEnumerable<Book>)null);

            var result = await _service.GetBooksByCategory(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetBooksByCategory_ShouldCallRepositoryOnce_WhenCalled()
        {
            var books = _fixture
                .CreateMany<Book>(2)
                .ToList();

            _bookRepositoryMock
                .Setup(r => r.GetBooksByCategory(1))
                .ReturnsAsync(books);

            await _service.GetBooksByCategory(1);

            _bookRepositoryMock.Verify(r => r.GetBooksByCategory(1), Times.Once);
        }

        [Theory]
        [InlineData("Test")]
        [InlineData("Book")]
        [InlineData("Programming")]
        public async Task Search_ShouldReturnListOfBooks_WhenBooksWithSearchedNameExist(string searchTerm)
        {
            var books = _fixture
                .Build<Book>()
                .With(b => b.Name, searchTerm)
                .CreateMany(2)
                .ToList();
            
            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(books);

            var result = await _service.Search(searchTerm);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_ShouldReturnNull_WhenBooksWithSearchedNameDoNotExist()
        {
            _bookRepositoryMock.Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync((IEnumerable<Book>)null);

            var result = await _service.Search("NonExistent");

            result.Should().BeNull();
        }

        [Fact]
        public async Task Search_ShouldCallRepositoryOnce_WhenCalled()
        {
            var books = _fixture.CreateMany<Book>().ToList();

            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(books);

            await _service.Search("Test");

            _bookRepositoryMock.Verify(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnListOfBooks_WhenBooksExist()
        {
            var books = _fixture.CreateMany<Book>(3).ToList();
            _bookRepositoryMock.Setup(r => r.SearchBookWithCategory("Test")).ReturnsAsync(books);

            var result = await _service.SearchBookWithCategory("Test");

            result.Should().NotBeNull();
            result.Should().BeOfType<List<Book>>();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldReturnNull_WhenBooksDoNotExist()
        {
            _bookRepositoryMock
                .Setup(r => r.SearchBookWithCategory(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Book>)null);

            var result = await _service.SearchBookWithCategory("NonExistent");

            result.Should().BeNull();
        }

        [Fact]
        public async Task SearchBookWithCategory_ShouldCallRepositoryOnce_WhenCalled()
        {
            var books = _fixture.CreateMany<Book>().ToList();
            _bookRepositoryMock
                .Setup(r => r.SearchBookWithCategory("Test"))
                .ReturnsAsync(books);

            await _service.SearchBookWithCategory("Test");

            _bookRepositoryMock.Verify(r => r.SearchBookWithCategory("Test"), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldAddBook_WhenBookNameDoesNotExist()
        {
            var book = _fixture.Create<Book>();
            
            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(new List<Book>());

            _bookRepositoryMock.Setup(r => r.Add(book));

            var result = await _service.Add(book);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().BeOfType<Book>();
        }

        [Fact]
        public async Task Add_ShouldNotAddBook_WhenBookNameAlreadyExists()
        {
            var book = _fixture.Create<Book>();

            var existingBooks = new List<Book> { book };

            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(existingBooks);

            var result = await _service.Add(book);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Add_ShouldCallRepositoryOnce_WhenBookIsValid()
        {
            var book = _fixture.Create<Book>();

            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(new List<Book>());

            _bookRepositoryMock.Setup(r => r.Add(book));

            await _service.Add(book);

            _bookRepositoryMock.Verify(r => r.Add(book), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldUpdateBook_WhenBookNameDoesNotExist()
        {
            var book = _fixture.Create<Book>();

            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(new List<Book>());

            _bookRepositoryMock.Setup(r => r.GetByIdAsNoTracking(book.Id)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.Update(book));

            var result = await _service.Update(book);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().BeOfType<Book>();
        }

        [Fact]
        public async Task Update_ShouldNotUpdateBook_WhenBookNameAlreadyExists()
        {
            var book = _fixture.Create<Book>();

            var conflictingBook = _fixture
                .Build<Book>()
                .With(b => b.Name, book.Name)
                .With(b => b.Id, book.Id + 1)
                .Create();

            var existingBooks = new List<Book> { conflictingBook };
            
            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(existingBooks);
            
            _bookRepositoryMock
                .Setup(r => r.GetByIdAsNoTracking(book.Id))
                .ReturnsAsync(book);

            var result = await _service.Update(book);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Update_ShouldCallRepositoryOnce_WhenBookIsValid()
        {
            var book = _fixture.Create<Book>();
            
            _bookRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>()))
                .ReturnsAsync(new List<Book>());
            
            _bookRepositoryMock
                .Setup(r => r.GetByIdAsNoTracking(book.Id))
                .ReturnsAsync(book);

            await _service.Update(book);

            _bookRepositoryMock.Verify(r => r.Update(book), Times.Once);
        }

        [Fact]
        public async Task Remove_ShouldReturnTrue_WhenBookCanBeRemoved()
        {
            var book = _fixture.Create<Book>();

            _bookRepositoryMock
                .Setup(r => r.GetById(book.Id))
                .ReturnsAsync(book);

            var result = await _service.Remove(book.Id);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Remove_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            _bookRepositoryMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync((Book)null);

            var result = await _service.Remove(999);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Remove_ShouldCallRepositoryOnce_WhenBookExists()
        {
            var book = _fixture.Create<Book>();
            
            _bookRepositoryMock
                .Setup(r => r.GetById(book.Id))
                .ReturnsAsync(book);

            await _service.Remove(book.Id);

            _bookRepositoryMock.Verify(r => r.Remove(book), Times.Once);
        }
    }
}
