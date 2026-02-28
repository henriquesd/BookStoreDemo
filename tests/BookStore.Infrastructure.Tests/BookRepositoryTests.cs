using AutoFixture;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using BookStore.Infrastructure.Repositories;
using BookStore.Infrastructure.Tests.Helpers;
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
            protected readonly Fixture _fixture;

            protected BookRepositoryTestsBase()
            {
                _options = BookStoreHelperTests.BookStoreDbContextOptionsSQLiteInMemory();
                BookStoreHelperTests.CreateDataBaseSQLiteInMemory(_options);
                _fixture = FixtureFactory.Create();
            }
        }

        public class GetAll : BookRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnAListOfBook_WhenBooksExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);

                // Act
                var books = await bookRepository.GetAll();

                // Assert
                books.Should().NotBeNull();
                books.Should().BeOfType<List<Book>>();
                books.Should().HaveCount(3);
            }

            [Fact]
            public async Task ShouldReturnAnEmptyList_WhenBooksDoNotExist()
            {
                // Arrange
                await BookStoreHelperTests.CleanDataBase(_options);
                await using var context = new BookStoreDbContext(_options);
                var bookRepository = new BookRepository(context);

                // Act
                var books = await bookRepository.GetAll();

                // Assert
                books.Should().NotBeNull();
                books.Should().BeEmpty();
                books.Should().BeOfType<List<Book>>();
            }

            [Fact]
            public async Task ShouldReturnAListOfBookWithCorrectValues_WhenBooksExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);

                // Act
                var bookList = await bookRepository.GetAll();

                // Assert
                bookList.Should().NotBeNull();
                bookList.Count().Should().Be(3);
                bookList.Should().AllSatisfy(b =>
                {
                    b.Name.Should().StartWith("Book Test");
                    b.Author.Should().StartWith("Author Test");
                });
            }
        }

        public class GetAllWithPagination : BookRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnAListOfPaginatedBook_WhenBooksExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var pageNumber = 1;
                var pageSize = 10;

                // Act
                var pagedResponse = await bookRepository.GetAllWithPagination(pageNumber, pageSize);

                // Assert
                pagedResponse.Should().NotBeNull();
                pagedResponse.Should().BeOfType<PagedResponse<Book>>();
                pagedResponse.PageNumber.Should().Be(pageNumber);
                pagedResponse.PageSize.Should().Be(pageSize);
            }

            [Fact]
            public async Task ShouldReturnAnEmptyList_WhenBooksDoNotExist()
            {
                // Arrange
                await BookStoreHelperTests.CleanDataBase(_options);
                await using var context = new BookStoreDbContext(_options);
                var bookRepository = new BookRepository(context);

                // Act
                var pagedResponse = await bookRepository.GetAllWithPagination(1, 10);

                // Assert
                pagedResponse.Should().NotBeNull();
                pagedResponse.Data.Should().BeEmpty();
                pagedResponse.Should().BeOfType<PagedResponse<Book>>();
            }

            [Fact]
            public async Task ShouldReturnPagedResponseOfBookWithCorrectValues_WhenBooksExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);

                // Act
                var pagedResponse = await bookRepository.GetAllWithPagination(1, 10);

                // Assert
                pagedResponse.Should().NotBeNull();
                pagedResponse.Should().BeOfType<PagedResponse<Book>>();
                pagedResponse.Data.Count().Should().Be(3);
                pagedResponse.Data.Should().AllSatisfy(b =>
                {
                    b.Name.Should().StartWith("Book Test");
                });
            }
        }

        public class GetById : BookRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnBookWithSearchedId_WhenBookWithSearchedIdExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var existingBookId = 2;

                // Act
                var book = await bookRepository.GetById(existingBookId);

                // Assert
                book.Should().NotBeNull();
                book.Should().BeOfType<Book>();
                book!.Id.Should().Be(existingBookId);
            }

            [Fact]
            public async Task ShouldReturnNull_WhenBookWithSearchedIdDoesNotExist()
            {
                // Arrange
                await BookStoreHelperTests.CleanDataBase(_options);
                await using var context = new BookStoreDbContext(_options);
                var bookRepository = new BookRepository(context);
                var nonExistentBookId = _fixture.Create<int>();

                // Act
                var book = await bookRepository.GetById(nonExistentBookId);

                // Assert
                book.Should().BeNull();
            }

            [Fact]
            public async Task ShouldReturnBookWithCorrectValues_WhenBookExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var existingBookId = 2;

                // Act
                var book = await bookRepository.GetById(existingBookId);

                // Assert
                book.Should().NotBeNull();
                book!.Id.Should().Be(existingBookId);
                book.Name.Should().Be("Book Test 2");
                book.Author.Should().Be("Author Test 2");
            }
        }

        public class GetBooksByCategory : BookRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnAListOfBook_WhenBooksWithSearchedCategoryExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var categoryId = 1;

                // Act
                var books = await bookRepository.GetBooksByCategory(categoryId);

                // Assert
                books.Should().NotBeNull();
                books.Should().BeOfType<List<Book>>();
                books.Should().HaveCount(2);
            }

            [Fact]
            public async Task ShouldReturnAnEmptyList_WhenNoBooksWithSearchedCategoryExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var nonExistentCategoryId = 999;

                // Act
                var books = await bookRepository.GetBooksByCategory(nonExistentCategoryId);
                var bookList = books as List<Book>;

                // Assert
                bookList.Should().NotBeNull();
                bookList.Should().BeEmpty();
                bookList.Should().BeOfType<List<Book>>();
            }

            [Fact]
            public async Task ShouldReturnAListOfBookWithSearchedCategory_WhenBooksWithSearchedCategoryExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var categoryId = 1;

                // Act
                var books = await bookRepository.GetBooksByCategory(categoryId);
                var bookList = books as List<Book>;

                // Assert
                bookList.Should().NotBeNull();
                bookList!.Count().Should().Be(2);
                bookList.Should().AllSatisfy(b => b.CategoryId.Should().Be(categoryId));
            }
        }

        public class SearchBookWithCategory : BookRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnOneBook_WhenOneBookWithSearchedValueExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var searchTerm = "Book Test 2";

                // Act
                var books = await bookRepository.SearchBookWithCategory(searchTerm);
                var bookList = books as List<Book>;

                // Assert
                bookList.Should().NotBeNull();
                bookList.Should().BeOfType<List<Book>>();
                bookList!.Count().Should().Be(1);
                bookList.First().Name.Should().Be(searchTerm);
            }

            [Fact]
            public async Task ShouldReturnAListOfBook_WhenBookWithSearchedValueExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var searchTerm = "Book Test";

                // Act
                var books = await bookRepository.SearchBookWithCategory(searchTerm);
                var bookList = books as List<Book>;

                // Assert
                bookList.Should().NotBeNull();
                bookList.Should().BeOfType<List<Book>>();
                bookList!.Count().Should().Be(3);
            }

            [Fact]
            public async Task ShouldReturnAnEmptyList_WhenNoBooksWithSearchedValueExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var bookRepository = new BookRepository(context);
                var searchTerm = _fixture.Create<string>();

                // Act
                var books = await bookRepository.SearchBookWithCategory(searchTerm);
                var bookList = books as List<Book>;

                // Assert
                bookList.Should().NotBeNull();
                bookList.Should().BeEmpty();
                bookList.Should().BeOfType<List<Book>>();
            }
        }
    }
}
