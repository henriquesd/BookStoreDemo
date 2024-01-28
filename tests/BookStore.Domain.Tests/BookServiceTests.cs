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
        public abstract class BookServiceTestsBase
        {
            protected readonly Fixture _fixture;
            protected readonly Mock<IBookRepository> _bookRepositoryMock;
            protected readonly BookService _bookService;

            protected BookServiceTestsBase()
            {
                _fixture = FixtureFactory.Create();
                _bookRepositoryMock = new Mock<IBookRepository>();
                _bookService = new BookService(_bookRepositoryMock.Object);
            }

            protected Book CreateBook()
            {
                return new Book()
                {
                    Id = 1,
                    Name = "Book Test",
                    Author = "Author Test",
                    Description = "Description Test",
                    Value = 10,
                    CategoryId = 1,
                    PublishDate = DateTime.MinValue.AddYears(40)
                };
            }

            protected List<Book> CreateBookList()
            {
                return new List<Book>()
                {
                    new Book()
                    {
                        Id = 1,
                        Name = "Book Test 1",
                        Author = "Author Test 1",
                        Description = "Description Test 1",
                        Value = 10,
                        CategoryId = 1
                    },
                    new Book()
                    {
                        Id = 2,
                        Name = "Book Test 2",
                        Author = "Author Test 2",
                        Description = "Description Test 2",
                        Value = 20,
                        CategoryId = 1
                    },
                    new Book()
                    {
                        Id = 3,
                        Name = "Book Test 3",
                        Author = "Author Test 3",
                        Description = "Description Test 3",
                        Value = 30,
                        CategoryId = 2
                    }
                };
            }
        }

        public class GetAll : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfBook_WhenBooksExist()
            {
                // Arrange
                var books = CreateBookList();

                _bookRepositoryMock.Setup(c => c.GetAll()).ReturnsAsync(books);

                // Act
                var result = await _bookService.GetAll();

                // Assert
                Assert.NotNull(result);
                Assert.IsType<List<Book>>(result);
            }

            [Fact]
            public async void ShouldReturnNull_WhenBooksDoNotExist()
            {
                // Arrange
                _bookRepositoryMock.Setup(c => c.GetAll())
                    .ReturnsAsync((List<Book>)null);

                // Act
                var result = await _bookService.GetAll();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async void ShouldCallGetAllFromRepository_OnlyOnce()
            {
                // Arrange
                _bookRepositoryMock.Setup(c => c.GetAll())
                    .ReturnsAsync(new List<Book>());

                // Act
                await _bookService.GetAll();

                // Assert
                _bookRepositoryMock.Verify(mock => mock.GetAll(), Times.Once);
            }
        }

        public class GetAllWithPagination : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAPagedResponseOfBooks_WhenBooksExist()
            {
                // Arrange
                var books = _fixture.Build<Book>()
                    .CreateMany()
                    .ToList();
                var pagedResponse = _fixture.Build<PagedResponse<Book>>()
                    .With(p => p.Data, books)
                    .Create();

                _bookRepositoryMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

                // Act
                var result = await _bookService.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>());

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<PagedResponse<Book>>();
            }

            [Fact]
            public async void ShouldReturnAnEmptyList_WhenBooksDoNotExist()
            {
                // Arrange
                var listBooks = new List<Book>();

                var pagedResponse = _fixture.Build<PagedResponse<Book>>()
                    .Without(p => p.Data)
                    .With(p => p.Data, listBooks)
                    .Create();

                _bookRepositoryMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

                // Act
                var result = await _bookService.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>());

                // Assert
                result.Should().NotBeNull();
                result.Data.Should().BeEmpty();
            }

            [Fact]
            public async void ShouldCallGetAllWithPaginationFromRepository_OnlyOnce()
            {
                // Arrange
                var pagedResponse = _fixture.Create<PagedResponse<Book>>();

                _bookRepositoryMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

                // Act
                var result = await _bookService.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>());

                // Assert
                _bookRepositoryMock.Verify(mock => mock.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            }
        }

        public class GetById : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnBook_WhenBookExist()
            {
                // Arrange
                var book = CreateBook();

                _bookRepositoryMock.Setup(c => c.GetById(book.Id))
                    .ReturnsAsync(book);

                // Act
                var result = await _bookService.GetById(book.Id);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<Book>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenBookDoesNotExist()
            {
                // Arrange
                _bookRepositoryMock.Setup(c => c.GetById(1))
                    .ReturnsAsync((Book)null);

                // Act
                var result = await _bookService.GetById(1);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallGetByIdFromRepository_OnlyOnce()
            {
                // Arrange
                _bookRepositoryMock.Setup(c => c.GetById(1))
                    .ReturnsAsync(new Book());

                // Act
                await _bookService.GetById(1);

                // Assert
                _bookRepositoryMock.Verify(mock => mock.GetById(1), Times.Once);
            }
        }

        public class GetBooksByCategory : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfBook_WhenBooksWithSearchedCategoryExist()
            {
                // Arrange
                var bookList = CreateBookList();

                _bookRepositoryMock.Setup(c => c.GetBooksByCategory(2))
                    .ReturnsAsync(bookList);

                // Act
                var result = await _bookService.GetBooksByCategory(2);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<List<Book>>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenBooksWithSearchedCategoryDoNotExist()
            {
                // Arrange
                _bookRepositoryMock.Setup(c => c.GetBooksByCategory(2))
                    .ReturnsAsync((IEnumerable<Book>)null);

                // Act
                var result = await _bookService.GetById(1);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallGetBooksByCategoryFromRepository_OnlyOnce()
            {
                // Arrange
                var bookList = CreateBookList();

                _bookRepositoryMock.Setup(c => c.GetBooksByCategory(2))
                    .ReturnsAsync(bookList);

                // Act
                await _bookService.GetBooksByCategory(2);

                // Assert
                _bookRepositoryMock.Verify(mock => mock.GetBooksByCategory(2), Times.Once);
            }
        }

        public class Search : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfBook_WhenBooksWithSearchedNameExist()
            {
                // Arrange
                var bookList = CreateBookList();
                var searchedBook = CreateBook();
                var bookName = searchedBook.Name;

                _bookRepositoryMock.Setup(c =>
                    c.Search(c => c.Name.Contains(bookName))).ReturnsAsync(bookList);

                // Act
                var result = await _bookService.Search(searchedBook.Name);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<List<Book>>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenBooksWithSearchedNameDoNotExist()
            {
                // Arrange
                var searchedBook = CreateBook();
                var bookName = searchedBook.Name;

                _bookRepositoryMock.Setup(c =>
                        c.Search(c => c.Name.Contains(bookName)))
                    .ReturnsAsync((IEnumerable<Book>)(null));

                // Act
                var result = await _bookService.Search(searchedBook.Name);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallSearchFromRepository_OnlyOnce()
            {
                // Arrange
                var bookList = CreateBookList();
                var searchedBook = CreateBook();
                var bookName = searchedBook.Name;

                _bookRepositoryMock.Setup(c =>
                        c.Search(c => c.Name.Contains(bookName)))
                    .ReturnsAsync(bookList);

                // Act
                await _bookService.Search(searchedBook.Name);

                // Assert
                _bookRepositoryMock.Verify(mock => mock.Search(c => c.Name.Contains(bookName)), Times.Once);
            }
        }

        public class SearchBookWithCategory : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfBook_WhenBooksWithSearchedCategoryExist()
            {
                // Arrange
                var bookList = CreateBookList();
                var searchedBook = CreateBook();

                _bookRepositoryMock.Setup(c =>
                    c.SearchBookWithCategory(searchedBook.Name))
                    .ReturnsAsync(bookList);

                // Act
                var result = await _bookService.SearchBookWithCategory(searchedBook.Name);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<List<Book>>(result);
            }

            [Fact]
            public async void ShouldReturnNull_WhenBooksWithSearchedCategoryDoNotExist()
            {
                // Arrange
                var searchedBook = CreateBook();

                _bookRepositoryMock.Setup(c =>
                    c.SearchBookWithCategory(searchedBook.Name))
                    .ReturnsAsync((IEnumerable<Book>)null);

                // Act
                var result = await _bookService.SearchBookWithCategory(searchedBook.Name);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallSearchBookWithCategoryFromRepository_OnlyOnce()
            {
                // Arrange
                var bookList = CreateBookList();
                var searchedBook = CreateBook();

                _bookRepositoryMock.Setup(c =>
                        c.SearchBookWithCategory(searchedBook.Name))
                    .ReturnsAsync(bookList);

                // Act
                await _bookService.SearchBookWithCategory(searchedBook.Name);

                // Assert
                _bookRepositoryMock.Verify(mock => mock.SearchBookWithCategory(searchedBook.Name), Times.Once);
            }
        }

        public class Add : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldAddBook_WhenBookNameDoesNotExist()
            {
                // Arrange
                var book = CreateBook();

                _bookRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == book.Name))
                    .ReturnsAsync(new List<Book>());
                _bookRepositoryMock.Setup(c => c.Add(book));

                // Act
                var result = await _bookService.Add(book);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<Book>();
            }

            [Fact]
            public async void ShouldNotAddBook_WhenBookNameAlreadyExist()
            {
                // Arrange
                var book = CreateBook();
                var bookList = new List<Book>() { book };

                _bookRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == book.Name))
                    .ReturnsAsync(bookList);

                // Act
                var result = await _bookService.Add(book);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallAddFromRepository_OnlyOnce()
            {
                // Arrange
                var book = CreateBook();

                _bookRepositoryMock.Setup(c =>
                        c.Search(c => c.Name == book.Name))
                    .ReturnsAsync(new List<Book>());
                _bookRepositoryMock.Setup(c => c.Add(book));

                // Act
                await _bookService.Add(book);

                // Assert
                _bookRepositoryMock.Verify(mock => mock.Add(book), Times.Once);
            }
        }

        public class Update : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldUpdateBook_WhenBookNameDoesNotExist()
            {
                // Arrange
                var book = CreateBook();

                _bookRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == book.Name && c.Id != book.Id))
                    .ReturnsAsync(new List<Book>());
                _bookRepositoryMock.Setup(c => c.Update(book));

                // Act
                var result = await _bookService.Update(book);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<Book>();
            }

            [Fact]
            public async void ShouldNotUpdateBook_WhenBookDoesNotExist()
            {
                // Arrange
                var book = CreateBook();
                var bookList = new List<Book>()
                {
                    new Book()
                    {
                        Id = 2,
                        Name = "Book Test 2",
                        Author = "Author Test 2"
                    }
                };

                _bookRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == book.Name && c.Id != book.Id))
                    .ReturnsAsync(bookList);

                // Act
                var result = await _bookService.Update(book);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallAddFromRepository_OnlyOnce()
            {
                // Arrange
                var book = CreateBook();

                _bookRepositoryMock.Setup(c =>
                        c.Search(c => c.Name == book.Name && c.Id != book.Id))
                    .ReturnsAsync(new List<Book>());

                // Act
                await _bookService.Update(book);

                // Assert
                _bookRepositoryMock.Verify(mock => mock.Update(book), Times.Once);
            }
        }

        public class Remove : BookServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnTrue_WhenBookCanBeRemoved()
            {
                // Arrange
                var book = CreateBook();

                // Act
                var result = await _bookService.Remove(book);

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public async void ShouldCallRemoveFromRepository_OnlyOnce()
            {
                // Arrange
                var book = CreateBook();

                // Act
                await _bookService.Remove(book);

                // Assert
                _bookRepositoryMock.Verify(mock => mock.Remove(book), Times.Once);
            }
        }
    }
}