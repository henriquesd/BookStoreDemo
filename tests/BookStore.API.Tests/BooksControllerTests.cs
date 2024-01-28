using AutoFixture;
using AutoMapper;
using BookStore.API.Controllers;
using BookStore.API.Dtos;
using BookStore.API.Dtos.Book;
using BookStore.API.Dtos.Category;
using BookStore.API.Tests.Helpers;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStore.API.Tests
{
    public class BooksControllerTests
    {
        public abstract class BooksControllerTestsBase
        {
            protected readonly Fixture _fixture;
            protected readonly BooksController _booksController;
            protected readonly Mock<IBookService> _bookServiceMock;
            protected readonly Mock<IMapper> _mapperMock;

            protected BooksControllerTestsBase()
            {
                _fixture = FixtureFactory.Create();
                _bookServiceMock = new Mock<IBookService>();
                _mapperMock = new Mock<IMapper>();
                _booksController = new BooksController(_mapperMock.Object, _bookServiceMock.Object);
            }

            protected Book CreateBook()
            {
                return new Book()
                {
                    Id = 2,
                    Name = "Book Test",
                    Author = "Author Test",
                    Description = "Description Test",
                    Value = 10,
                    CategoryId = 1,
                    PublishDate = DateTime.MinValue.AddYears(40),
                    Category = new Category()
                    {
                        Id = 1,
                        Name = "Category Test"
                    }
                };
            }

            protected BookResultDto MapModelToBookResultDto(Book book)
            {
                var bookDto = new BookResultDto()
                {
                    Id = book.Id,
                    Name = book.Name,
                    Author = book.Author,
                    Description = book.Description,
                    PublishDate = book.PublishDate,
                    Value = book.Value,
                    CategoryId = book.CategoryId
                };
                return bookDto;
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
                    Id = 1,
                    Name = "Book Test 2",
                    Author = "Author Test 2",
                    Description = "Description Test 2",
                    Value = 20,
                    CategoryId = 1
                },
                new Book()
                {
                    Id = 1,
                    Name = "Book Test 3",
                    Author = "Author Test 3",
                    Description = "Description Test 3",
                    Value = 30,
                    CategoryId = 2
                }
            };
            }

            protected List<BookResultDto> MapModelToBookResultListDto(List<Book> books)
            {
                var listBooks = new List<BookResultDto>();

                foreach (var item in books)
                {
                    var book = new BookResultDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Author = item.Author,
                        Description = item.Description,
                        PublishDate = item.PublishDate,
                        Value = item.Value,
                        CategoryId = item.CategoryId
                    };
                    listBooks.Add(book);
                }
                return listBooks;
            }
        }

        public class GetAll : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBooksExist()
            {
                // Arrange
                var books = CreateBookList();
                var dtoExpected = MapModelToBookResultListDto(books);

                _bookServiceMock.Setup(c => c.GetAll()).ReturnsAsync(books);
                _mapperMock.Setup(m => m.Map<IEnumerable<BookResultDto>>(It.IsAny<List<Book>>())).Returns(dtoExpected);

                // Act
                var result = await _booksController.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnOk_WhenDoesNotExistAnyBook()
            {
                // Arrange
                var books = new List<Book>();
                var dtoExpected = MapModelToBookResultListDto(books);

                _bookServiceMock.Setup(c => c.GetAll()).ReturnsAsync(books);
                _mapperMock.Setup(m => m.Map<IEnumerable<BookResultDto>>(It.IsAny<List<Book>>())).Returns(dtoExpected);

                // Act
                var result = await _booksController.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldCallGetAllFromService_OnlyOnce()
            {
                // Arrange
                var books = CreateBookList();
                var dtoExpected = MapModelToBookResultListDto(books);

                _bookServiceMock.Setup(c => c.GetAll()).ReturnsAsync(books);
                _mapperMock.Setup(m => m.Map<IEnumerable<BookResultDto>>(It.IsAny<List<Book>>())).Returns(dtoExpected);

                // Act
                await _booksController.GetAll();

                // Assert
                _bookServiceMock.Verify(mock => mock.GetAll(), Times.Once);
            }
        }

        public class GetAllWithPagination : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBooksExist()
            {
                // Arrange
                var books = _fixture.Build<Book>()
                   .CreateMany()
                   .ToList();

                var pagedResponse = _fixture.Build<PagedResponse<Book>>()
                    .With(p => p.Data, books)
                    .Create();

                var listBookResultDto = _fixture.Build<BookResultDto>().CreateMany().ToList();

                var pagedResponseDto = new PagedResponseDto<BookResultDto>
                {
                    Data = listBookResultDto,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalRecords = 1,
                    TotalPages = 1
                };

                _bookServiceMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);
                _mapperMock.Setup(m => m.Map<PagedResponseDto<BookResultDto>>(It.IsAny<PagedResponse<Book>>())).Returns(pagedResponseDto);


                // Act
                var result = await _booksController.GetAllWithPagination();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnOk_WhenDoesNotExistAnyBook()
            {
                // Arrange
                var pagedResponse = _fixture.Build<PagedResponse<Book>>()
                    .Without(p => p.Data)
                    .Create();

                var listBookResultDto = _fixture.Build<BookResultDto>().CreateMany().ToList();
                
                var pagedResponseDto = new PagedResponseDto<BookResultDto>
                {
                    Data = null,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalRecords = 1,
                    TotalPages = 1
                };

                _bookServiceMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);
                _mapperMock.Setup(m => m.Map<PagedResponseDto<BookResultDto>>(It.IsAny<PagedResponse<Book>>())).Returns(pagedResponseDto);

                // Act
                var result = await _booksController.GetAllWithPagination();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldCallGetAllWithPaginationFromService_OnlyOnce()
            {
                // Arrange
                var books = _fixture.Build<Book>()
                    .CreateMany()
                    .ToList();

                var pagedResponse = _fixture.Build<PagedResponse<Book>>()
                    .With(p => p.Data, books)
                    .Create();

                var listBookResultDto = _fixture.Build<BookResultDto>().CreateMany().ToList();

                var pagedResponseDto = new PagedResponseDto<BookResultDto>
                {
                    Data = listBookResultDto,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalRecords = 1,
                    TotalPages = 1
                };

                _bookServiceMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);
                _mapperMock.Setup(m => m.Map<PagedResponseDto<BookResultDto>>(It.IsAny<PagedResponse<Book>>())).Returns(pagedResponseDto);

                // Act
                await _booksController.GetAllWithPagination();

                // Assert
                _bookServiceMock.Verify(mock => mock.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            }

            [Theory]
            [InlineData(0, 10)]
            [InlineData(1, 0)]
            [InlineData(-1, 10)]
            [InlineData(1, -1)]
            public async void ShouldReturnBadRequest_WhenPaginationParametersAreInvalid(int pageNumber, int pageSize)
            {
                // Act
                var result = await _booksController.GetAllWithPagination(pageNumber, pageSize);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }
        }

        public class GetById : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBookExists()
            {
                // Arrange
                var book = CreateBook();
                var dtoExpected = MapModelToBookResultDto(book);

                _bookServiceMock.Setup(c => c.GetById(2)).ReturnsAsync(book);
                _mapperMock.Setup(m => m.Map<BookResultDto>(It.IsAny<Book>())).Returns(dtoExpected);

                // Act
                var result = await _booksController.GetById(2);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenBookDoesNotExists()
            {
                // Arrange
                _bookServiceMock.Setup(c => c.GetById(2)).ReturnsAsync((Book)null);

                // Act
                var result = await _booksController.GetById(2);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async void ShouldCallGetByIdFromService_OnlyOnce()
            {
                // Arrange
                var book = CreateBook();
                var dtoExpected = MapModelToBookResultDto(book);

                _bookServiceMock.Setup(c => c.GetById(2)).ReturnsAsync(book);
                _mapperMock.Setup(m => m.Map<BookResultDto>(It.IsAny<Book>())).Returns(dtoExpected);

                // Act
                await _booksController.GetById(2);

                // Assert
                _bookServiceMock.Verify(mock => mock.GetById(2), Times.Once);
            }
        }

        public class GetBooksByCategory : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBookWithSearchedCategoryExist()
            {
                // Arrange
                var bookList = CreateBookList();
                var book = CreateBook();
                var dtoExpected = MapModelToBookResultListDto(bookList);

                _bookServiceMock.Setup(c => c.GetBooksByCategory(book.CategoryId)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<BookResultDto>>(It.IsAny<IEnumerable<Book>>())).Returns(dtoExpected);

                // Act
                var result = await _booksController.GetBooksByCategory(book.CategoryId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenBookWithSearchedCategoryDoesNotExist()
            {
                // Arrange
                var book = CreateBook();
                var dtoExpected = MapModelToBookResultDto(book);

                _bookServiceMock.Setup(c => c.GetBooksByCategory(book.CategoryId)).ReturnsAsync(new List<Book>());
                _mapperMock.Setup(m => m.Map<BookResultDto>(It.IsAny<Book>())).Returns(dtoExpected);

                // Act
                var result = await _booksController.GetBooksByCategory(book.CategoryId);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async void ShouldCallGetBooksByCategoryFromService_OnlyOnce()
            {
                // Arrange
                var bookList = CreateBookList();
                var book = CreateBook();
                var dtoExpected = MapModelToBookResultListDto(bookList);

                _bookServiceMock.Setup(c => c.GetBooksByCategory(book.CategoryId)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<BookResultDto>>(It.IsAny<IEnumerable<Book>>())).Returns(dtoExpected);

                // Act
                await _booksController.GetBooksByCategory(book.CategoryId);

                // Assert
                _bookServiceMock.Verify(mock => mock.GetBooksByCategory(book.CategoryId), Times.Once);
            }
        }

        public class Add : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBookIsAdded()
            {
                // Arrange
                var book = CreateBook();
                var bookAddDto = new BookAddDto() { Name = book.Name };
                var bookResultDto = MapModelToBookResultDto(book);

                _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookAddDto>())).Returns(book);
                _bookServiceMock.Setup(c => c.Add(book)).ReturnsAsync(book);
                _mapperMock.Setup(m => m.Map<BookResultDto>(It.IsAny<Book>())).Returns(bookResultDto);

                // Act
                var result = await _booksController.Add(bookAddDto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenModelStateIsInvalid()
            {
                // Arrange
                var bookAddDto = new BookAddDto();
                _booksController.ModelState.AddModelError("Name", "The field name is required");

                // Act
                var result = await _booksController.Add(bookAddDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenBookResultIsNull()
            {
                // Arrange
                var book = CreateBook();
                var bookAddDto = new BookAddDto() { Name = book.Name };

                _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookAddDto>())).Returns(book);
                _bookServiceMock.Setup(c => c.Add(book)).ReturnsAsync((Book)null);

                // Act
                var result = await _booksController.Add(bookAddDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldCallAddFromService_OnlyOnce()
            {
                // Arrange
                var book = CreateBook();
                var bookAddDto = new BookAddDto() { Name = book.Name };

                _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookAddDto>())).Returns(book);
                _bookServiceMock.Setup(c => c.Add(book)).ReturnsAsync(book);

                // Act
                await _booksController.Add(bookAddDto);

                // Assert
                _bookServiceMock.Verify(mock => mock.Add(book), Times.Once);
            }
        }

        public class Update : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBookIsUpdatedCorrectly()
            {
                // Arrange
                var book = CreateBook();
                var bookEditDto = new BookEditDto() { Id = book.Id, Name = "Test" };

                _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookEditDto>())).Returns(book);
                _bookServiceMock.Setup(c => c.GetById(book.Id)).ReturnsAsync(book);
                _bookServiceMock.Setup(c => c.Update(book)).ReturnsAsync(book);

                // Act
                var result = await _booksController.Update(bookEditDto.Id, bookEditDto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenBookIdIsDifferentThenParameterId()
            {
                // Arrange
                var bookEditDto = new BookEditDto() { Id = 1, Name = "Test" };

                // Act
                var result = await _booksController.Update(2, bookEditDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenModelStateIsInvalid()
            {
                // Arrange
                var bookEditDto = new BookEditDto() { Id = 1 };
                _booksController.ModelState.AddModelError("Name", "The field name is required");

                // Act
                var result = await _booksController.Update(1, bookEditDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldCallUpdateFromService_OnlyOnce()
            {
                // Arrange
                var book = CreateBook();
                var bookEditDto = new BookEditDto() { Id = book.Id, Name = "Test" };

                _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookEditDto>())).Returns(book);
                _bookServiceMock.Setup(c => c.GetById(book.Id)).ReturnsAsync(book);
                _bookServiceMock.Setup(c => c.Update(book)).ReturnsAsync(book);

                // Act
                await _booksController.Update(bookEditDto.Id, bookEditDto);

                // Assert
                _bookServiceMock.Verify(mock => mock.Update(book), Times.Once);
            }
        }

        public class Remove : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBookIsRemoved()
            {
                // Arrange
                var book = CreateBook();
                _bookServiceMock.Setup(c => c.GetById(book.Id)).ReturnsAsync(book);
                _bookServiceMock.Setup(c => c.Remove(book)).ReturnsAsync(true);

                // Act
                var result = await _booksController.Remove(book.Id);

                // Assert
                result.Should().BeOfType<OkResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenBookDoesNotExist()
            {
                // Arrange
                var book = CreateBook();
                _bookServiceMock.Setup(c => c.GetById(book.Id)).ReturnsAsync((Book)null);

                // Act
                var result = await _booksController.Remove(book.Id);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async void ShouldCallRemoveFromService_OnlyOnce()
            {
                // Arrange
                var book = CreateBook();
                _bookServiceMock.Setup(c => c.GetById(book.Id)).ReturnsAsync(book);
                _bookServiceMock.Setup(c => c.Remove(book)).ReturnsAsync(true);

                // Act
                await _booksController.Remove(book.Id);

                // Assert
                _bookServiceMock.Verify(mock => mock.Remove(book), Times.Once);
            }
        }

        public class Search : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBookWithSearchedNameExist()
            {
                // Arrange
                var bookList = CreateBookList();
                var book = CreateBook();

                _bookServiceMock.Setup(c => c.Search(book.Name)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<List<Book>>(It.IsAny<IEnumerable<Book>>())).Returns(bookList);

                // Act
                var result = await _booksController.Search(book.Name);
                var actual = (OkObjectResult)result.Result;

                // Assert
                actual.Should().NotBeNull();
                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenBookWithSearchedNameDoesNotExist()
            {
                // Arrange
                var book = CreateBook();
                var bookList = new List<Book>();

                var dtoExpected = MapModelToBookResultDto(book);
                book.Name = dtoExpected.Name;

                _bookServiceMock.Setup(c => c.Search(book.Name)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<Book>>(It.IsAny<Book>())).Returns(bookList);

                // Act
                var result = await _booksController.Search(book.Name);
                var actual = (NotFoundObjectResult)result.Result;

                // Assert
                actual.Should().NotBeNull();
                actual.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async void ShouldCallSearchFromService_OnlyOnce()
            {
                // Arrange
                var bookList = CreateBookList();
                var book = CreateBook();

                _bookServiceMock.Setup(c => c.Search(book.Name)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<List<Book>>(It.IsAny<IEnumerable<Book>>())).Returns(bookList);

                // Act
                await _booksController.Search(book.Name);

                // Assert
                _bookServiceMock.Verify(mock => mock.Search(book.Name), Times.Once);
            }
        }

        public class SearchBookWithCategory : BooksControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenBookWithSearchedValueExist()
            {
                // Arrange
                var bookList = CreateBookList();
                var book = CreateBook();
                var bookResultList = MapModelToBookResultListDto(bookList);

                _bookServiceMock.Setup(c => c.SearchBookWithCategory(book.Name)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<Book>>(It.IsAny<List<Book>>())).Returns(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<BookResultDto>>(It.IsAny<List<Book>>())).Returns(bookResultList);

                // Act
                var result = await _booksController.SearchBookWithCategory(book.Name);
                var actual = (OkObjectResult)result.Result;

                // Assert
                actual.Should().NotBeNull();
                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenBookWithSearchedValueDoesNotExist()
            {
                // Arrange
                var book = CreateBook();
                var bookList = new List<Book>();

                _bookServiceMock.Setup(c => c.SearchBookWithCategory(book.Name)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<Book>>(It.IsAny<List<Book>>())).Returns(bookList);

                // Act
                var result = await _booksController.SearchBookWithCategory(book.Name);
                var actual = (NotFoundObjectResult)result.Result;

                // Assert
                actual.Value.Equals("None book was founded");
                actual.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async void ShouldCallSearchBookWithCategoryFromService_OnlyOnce()
            {
                // Arrange
                var bookList = CreateBookList();
                var book = CreateBook();
                var bookResultList = MapModelToBookResultListDto(bookList);

                _bookServiceMock.Setup(c => c.SearchBookWithCategory(book.Name)).ReturnsAsync(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<Book>>(It.IsAny<List<Book>>())).Returns(bookList);
                _mapperMock.Setup(m => m.Map<IEnumerable<BookResultDto>>(It.IsAny<List<Book>>())).Returns(bookResultList);

                // Act
                await _booksController.SearchBookWithCategory(book.Name);

                // Assert
                _bookServiceMock.Verify(mock => mock.SearchBookWithCategory(book.Name), Times.Once);
            }
        }
    }
}