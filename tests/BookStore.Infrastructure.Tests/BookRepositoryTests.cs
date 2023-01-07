using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using BookStore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.Infrastructure.Tests
{
    public class BookRepositoryTests
    {
        public abstract class BookRepositoryTestsBase
        {
            protected readonly DbContextOptions<BookStoreDbContext> _options;

            protected BookRepositoryTestsBase()
            {
                // Use this when using a SQLite InMemory database
                _options = BookStoreHelperTests.BookStoreDbContextOptionsSQLiteInMemory();
                BookStoreHelperTests.CreateDataBaseSQLiteInMemory(_options);

                // Use this when using a EF Core InMemory database
                //_options = BookStoreHelperTests.BookStoreDbContextOptionsEfCoreInMemory();
                //BookStoreHelperTests.CreateDataBaseEfCoreInMemory(_options);
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
                    CategoryId = 1,
                    PublishDate = new DateTime(2020, 1, 1, 0, 0, 0, 0),
                    Category = new Category()
                    {
                        Id = 1,
                        Name = "Category Test 1"
                    }
                },
                new Book()
                {
                    Id = 2,
                    Name = "Book Test 2",
                    Author = "Author Test 2",
                    Description = "Description Test 2",
                    Value = 20,
                    CategoryId = 1,
                    PublishDate = new DateTime(2020, 2, 2, 0, 0, 0, 0),
                    Category = new Category()
                    {
                        Id = 1,
                        Name = "Category Test 1"
                    }
                },
                new Book()
                {
                    Id = 3,
                    Name = "Book Test 3",
                    Author = "Author Test 3",
                    Description = "Description Test 3",
                    Value = 30,
                    CategoryId = 3,
                    PublishDate = new DateTime(2020, 3, 3, 0, 0, 0, 0),
                    Category = new Category()
                    {
                        Id = 3,
                        Name = "Category Test 3"
                    }
                }
            };
            }
        }

        public class GetAll : BookRepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfBook_WhenBooksExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    // Act
                    var books = await bookRepository.GetAll();

                    // Assert
                    books.Should().NotBeNull();
                    books.Should().BeOfType<List<Book>>();
                }
            }

            [Fact]
            public async void ShouldReturnAnEmptyList_WhenBooksDoNotExist()
            {
                await BookStoreHelperTests.CleanDataBase(_options);

                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    // Act
                    var books = await bookRepository.GetAll();

                    // Assert
                    books.Should().NotBeNull();
                    books.Should().BeEmpty();
                    books.Should().BeOfType<List<Book>>();
                }
            }

            [Fact]
            public async void ShouldReturnAListOfBookWithCorrectValues_WhenBooksExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    var expectedBooks = CreateBookList();

                    // Act
                    var bookList = await bookRepository.GetAll();

                    // Assert
                    bookList.Should().NotBeNull();
                    bookList.Count().Should().Be(3);
                    bookList.Should().BeEquivalentTo(expectedBooks, options =>
                        options.Excluding(x => x.Category.Books));
                }
            }
        }

        public class GetById : BookRepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnBookWithSearchedId_WhenBookWithSearchedIdExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    // Act
                    var book = await bookRepository.GetById(2);

                    // Assert
                    book.Should().NotBeNull();
                    book.Should().BeOfType<Book>();
                }
            }

            [Fact]
            public async void ShouldReturnNull_WhenBookWithSearchedIdDoesNotExist()
            {
                await BookStoreHelperTests.CleanDataBase(_options);

                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    // Act
                    var book = await bookRepository.GetById(1);

                    // Assert
                    book.Should().BeNull();
                }
            }

            [Fact]
            public async void ShouldReturnBookWithCorrectValues_WhenBookExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);
                    var expectedBooks = CreateBookList();

                    // Act
                    var book = await bookRepository.GetById(2);

                    // Assert
                    book.Should().BeEquivalentTo(expectedBooks[1], options =>
                       options.Excluding(x => x.Category.Books));
                }
            }
        }

        public class GetBooksByCategory : BookRepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfBook_WhenBooksWithSearchedCategoryExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    // Act
                    var books = await bookRepository.GetBooksByCategory(1);

                    // Assert
                    books.Should().NotBeNull();
                    books.Should().BeOfType<List<Book>>();
                }
            }

            [Fact]
            public async void ShouldReturnAnEmptyList_WhenNoBooksWithSearchedCategoryExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    // Act
                    var books = await bookRepository.GetBooksByCategory(4);
                    var bookList = books as List<Book>;

                    // Assert
                    bookList.Should().NotBeNull();
                    bookList.Should().BeEmpty();
                    bookList.Should().BeOfType<List<Book>>();
                }
            }

            [Fact]
            public async void ShouldReturnAListOfBookWithSearchedCategory_WhenBooksWithSearchedCategoryExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    var expectedBooks = CreateBookList();
                    var expectedBooksWithCategory = expectedBooks.Where(b => b.Category.Id == 1).ToList();

                    // Act
                    var books = await bookRepository.GetBooksByCategory(1);
                    var bookList = books as List<Book>;

                    // Assert
                    bookList.Should().NotBeNull();
                    bookList.Count().Should().Be(2);
                    bookList.Should().BeEquivalentTo(expectedBooksWithCategory, options =>
                      options.Excluding(x => x.Category));
                }
            }
        }

        public class SearchBookWithCategory : BookRepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnOneBook_WhenOneBookWithSearchedValueExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);
                    var expectedBook = CreateBookList();

                    // Act
                    var books = await bookRepository.SearchBookWithCategory(expectedBook[1].Name);
                    var bookList = books as List<Book>;

                    // Assert
                    bookList.Should().NotBeNull();
                    bookList.Should().BeOfType<List<Book>>();
                    bookList.Count().Should().Be(1);
                    bookList.FirstOrDefault().Should().BeEquivalentTo(expectedBook[1], options =>
                      options.Excluding(x => x.Category.Books));
                }
            }

            [Fact]
            public async void ShouldReturnAListOfBook_WhenBookWithSearchedValueExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);
                    var expectedBooks = CreateBookList();

                    // Act
                    var books = await bookRepository.SearchBookWithCategory("Book Test");
                    var bookList = books as List<Book>;

                    // Assert
                    bookList.Should().NotBeNull();
                    bookList.Should().BeOfType<List<Book>>();
                    bookList.Count().Should().Be(expectedBooks.Count);
                    bookList.Should().BeEquivalentTo(expectedBooks, options =>
                      options.Excluding(x => x.Category.Books));

                }
            }

            [Fact]
            public async void ShouldReturnAnEmptyList_WhenNoBooksWithSearchedValueExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var bookRepository = new BookRepository(context);

                    // Act
                    var books = await bookRepository.SearchBookWithCategory("Testt");
                    var bookList = books as List<Book>;

                    // Assert
                    bookList.Should().NotBeNull();
                    bookList.Should().BeEmpty();
                    bookList.Should().BeOfType<List<Book>>();
                }
            }
        }
    }
}