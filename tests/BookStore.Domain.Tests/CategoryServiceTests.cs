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
    public class CategoryServiceTests
    {
        public abstract class CategoryServiceTestsBase
        {
            protected readonly Fixture _fixture;
            protected readonly Mock<ICategoryRepository> _categoryRepositoryMock;
            protected readonly Mock<IBookService> _bookService;
            protected readonly CategoryService _categoryService;

            protected CategoryServiceTestsBase()
            {
                _fixture = FixtureFactory.Create();
                _categoryRepositoryMock = new Mock<ICategoryRepository>();
                _bookService = new Mock<IBookService>();
                _categoryService = new CategoryService(_categoryRepositoryMock.Object, _bookService.Object);
            }
        }

        public class GetAll : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfCategories_WhenCategoriesExist()
            {
                // Arrange
                var categories = _fixture.Create<List<Category>>();

                _categoryRepositoryMock.Setup(c =>
                    c.GetAll()).ReturnsAsync(categories);

                // Act
                var result = await _categoryService.GetAll();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<List<Category>>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenCategoriesDoNotExist()
            {
                // Arrange
                _categoryRepositoryMock.Setup(c => c.GetAll()).ReturnsAsync((List<Category>)null);

                // Act
                var result = await _categoryService.GetAll();

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallGetAllFromRepository_OnlyOnce()
            {
                // Arrange
                _categoryRepositoryMock.Setup(c =>
                    c.GetAll()).ReturnsAsync((List<Category>)null);

                // Act
                await _categoryService.GetAll();

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.GetAll(), Times.Once);
            }
        }

        public class GetAllWithPagination : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfCategories_WhenCategoriesExist()
            {
                // Arrange
                var categories = _fixture.Build<Category>()
                    .CreateMany()
                    .ToList();
                var pagedResponse = _fixture.Build<PagedResponse<Category>>()
                    .With(p => p.Data, categories)
                    .Create();

                _categoryRepositoryMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

                // Act
                var result = await _categoryService.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>());

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<PagedResponse<Category>>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenCategoriesDoNotExist()
            {
                // Arrange
                var pagedResponse = _fixture.Build<PagedResponse<Category>>()
                    .Without(p => p.Data)
                    .Do(p => p.Data = new List<Category>())
                    .Create();

                _categoryRepositoryMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

                // Act
                var result = await _categoryService.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>());

                // Assert
                result.Should().NotBeNull();
                result.Data.Should().BeEmpty();
            }

            [Fact]
            public async void ShouldCallGetAllFromRepository_OnlyOnce()
            {
                // Arrange
                var pagedResponse = _fixture.Create<PagedResponse<Category>>();

                _categoryRepositoryMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

                // Act
                var result = await _categoryService.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>());

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            }
        }

        public class GetById : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnCategory_WhenCategoryExist()
            {
                // Arrange
                var category = _fixture.Create<Category>();

                _categoryRepositoryMock.Setup(c =>
                    c.GetById(category.Id)).ReturnsAsync(category);

                // Act
                var result = await _categoryService.GetById(category.Id);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<Category>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenCategoryDoesNotExist()
            {
                // Arrange
                _categoryRepositoryMock.Setup(c =>
                    c.GetById(1)).ReturnsAsync((Category)null);

                // Act
                var result = await _categoryService.GetById(1);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallGetByIdFromRepository_OnlyOnce()
            {
                // Assert
                _categoryRepositoryMock.Setup(c =>
                    c.GetById(1)).ReturnsAsync((Category)null);

                // Act
                await _categoryService.GetById(1);

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.GetById(1), Times.Once);
            }
        }

        public class Add : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldAddCategory_WhenCategoryNameDoesNotExist()
            {
                // Assert
                var category = _fixture.Create<Category>();

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == category.Name))
                    .ReturnsAsync(new List<Category>());
                _categoryRepositoryMock.Setup(c => c.Add(category));

                // Act
                var result = await _categoryService.Add(category);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<OperationResult<Category>>();
                result.Success.Should().BeTrue();
            }

            [Fact]
            public async void ShouldNotAddCategory_WhenCategoryNameAlreadyExist()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var categoryList = _fixture.Create<List<Category>>();

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == category.Name)).ReturnsAsync(categoryList);

                // Act
                var result = await _categoryService.Add(category);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
            }

            [Fact]
            public async void ShouldCallAddFromRepository_OnlyOnce()
            {
                // Arrange
                var category = _fixture.Create<Category>();

                _categoryRepositoryMock.Setup(c =>
                        c.Search(c => c.Name == category.Name))
                    .ReturnsAsync(new List<Category>());
                _categoryRepositoryMock.Setup(c => c.Add(category));

                // Act
                await _categoryService.Add(category);

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.Add(category), Times.Once);
            }
        }

        public class Update : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldUpdateCategory_WhenCategoryNameDoesNotExist()
            {
                // Arrange
                var category = _fixture.Create<Category>();

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == category.Name && c.Id != category.Id))
                    .ReturnsAsync(new List<Category>());
                _categoryRepositoryMock.Setup(c => c.Update(category));

                // Act
                var result = await _categoryService.Update(category);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Payload.Should().NotBeNull();
                result.Should().BeOfType<OperationResult<Category>>();
            }

            [Fact]
            public async void ShouldNotUpdateCategory_WhenCategoryDoesNotExist()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var categoryList = _fixture.CreateMany<Category>();

                _categoryRepositoryMock.Setup(c =>
                        c.Search(c => c.Name == category.Name && c.Id != category.Id))
                    .ReturnsAsync(categoryList);

                // Act
                var result = await _categoryService.Update(category);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.Payload.Should().NotBeNull();
            }


            [Fact]
            public async void ShouldNotUpdateCategory_WhenCategoryNameIsAlreadyBeingUsed()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var categoryList = _fixture.CreateMany<Category>();

                _categoryRepositoryMock.Setup(c =>
                        c.Search(c => c.Name == category.Name && c.Id != category.Id))
                    .ReturnsAsync(categoryList);

                // Act
                var result = await _categoryService.Update(category);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.Payload.Should().NotBeNull();
            }

            [Fact]
            public async void ShouldCallUpdateFromRepository_OnlyOnce()
            {
                // Arrange
                var category = _fixture.Create<Category>();

                _categoryRepositoryMock.Setup(c =>
                        c.Search(c => c.Name == category.Name && c.Id != category.Id))
                    .ReturnsAsync(new List<Category>());

                // Act
                await _categoryService.Update(category);

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.Update(category), Times.Once);
            }
        }

        public class Remove : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldRemoveCategory_WhenCategoryDoNotHaveRelatedBooks()
            {
                // Arrange
                var category = _fixture.Create<Category>();

                _bookService.Setup(b =>
                    b.GetBooksByCategory(category.Id)).ReturnsAsync(new List<Book>());

                // Act
                var result = await _categoryService.Remove(category);

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public async void ShouldNotRemoveCategory_WhenCategoryHasRelatedBooks()
            {
                // Arrange
                var category = _fixture.Create<Category>();

                var books = _fixture.Build<Book>()
                    .With(p => p.CategoryId, category.Id)
                    .CreateMany();

                _bookService.Setup(b => b.GetBooksByCategory(category.Id)).ReturnsAsync(books);

                // Act
                var result = await _categoryService.Remove(category);

                // Assert
                result.Should().BeFalse();
            }

            [Fact]
            public async void ShouldCallRemoveFromRepository_OnlyOnce()
            {
                // Arrange
                var category = _fixture.Create<Category>();

                _bookService.Setup(b =>
                    b.GetBooksByCategory(category.Id)).ReturnsAsync(new List<Book>());

                // Act
                await _categoryService.Remove(category);

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.Remove(category), Times.Once);
            }
        }

        public class Search : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfCategory_WhenCategoriesWithSearchedNameExist()
            {
                // Arrange
                var categoryList = _fixture.Create<List<Category>>();
                var categoryName = _fixture.Create<string>();

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name.Contains(categoryName)))
                    .ReturnsAsync(categoryList);

                // Act
                var result = await _categoryService.Search(categoryName);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<List<Category>>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenCategoriesWithSearchedNameDoNotExist()
            {
                // Arrange
                var searchedCategory = _fixture.Create<Category>();
                var categoryName = _fixture.Create<string>();

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name.Contains(categoryName)))
                    .ReturnsAsync((IEnumerable<Category>)(null));

                // Act
                var result = await _categoryService.Search(categoryName);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallSearchFromRepository_OnlyOnce()
            {
                // Arrange
                var categoryList = _fixture.Create<List<Category>>();
                var categoryName = _fixture.Create<string>();

                _categoryRepositoryMock.Setup(c =>
                        c.Search(c => c.Name.Contains(categoryName)))
                    .ReturnsAsync(categoryList);

                // Act
                await _categoryService.Search(categoryName);

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.Search(c => c.Name.Contains(categoryName)), Times.Once);
            }
        }
    }
}