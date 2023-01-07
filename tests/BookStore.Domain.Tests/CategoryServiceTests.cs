using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.Domain.Tests
{
    public class CategoryServiceTests
    {
        public abstract class CategoryServiceTestsBase
        {
            protected readonly Mock<ICategoryRepository> _categoryRepositoryMock;
            protected readonly Mock<IBookService> _bookService;
            protected readonly CategoryService _categoryService;

            protected CategoryServiceTestsBase()
            {
                _categoryRepositoryMock = new Mock<ICategoryRepository>();
                _bookService = new Mock<IBookService>();
                _categoryService = new CategoryService(_categoryRepositoryMock.Object, _bookService.Object);
            }

            protected Category CreateCategory()
            {
                return new Category()
                {
                    Id = 1,
                    Name = "Category Name 1"
                };
            }

            protected List<Category> CreateCategoryList()
            {
                return new List<Category>()
                {
                    new Category()
                    {
                        Id = 1,
                        Name = "Category Name 1"
                    },
                    new Category()
                    {
                        Id = 2,
                        Name = "Category Name 2"
                    },
                    new Category()
                    {
                        Id = 3,
                        Name = "Category Name 3"
                    }
                };
            }
        }

        public class GetAll : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfCategories_WhenCategoriesExist()
            {
                // Arrange
                var categories = CreateCategoryList();

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
                _categoryRepositoryMock.Setup(c =>
                    c.GetAll()).ReturnsAsync((List<Category>)null);

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

        public class GetById : CategoryServiceTestsBase
        {
            [Fact]
            public async void ShouldReturnCategory_WhenCategoryExist()
            {
                // Arrange
                var category = CreateCategory();

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
                var category = CreateCategory();

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == category.Name))
                    .ReturnsAsync(new List<Category>());
                _categoryRepositoryMock.Setup(c => c.Add(category));

                // Act
                var result = await _categoryService.Add(category);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<Category>();
            }

            [Fact]
            public async void ShouldNotAddCategory_WhenCategoryNameAlreadyExist()
            {
                // Arrange
                var category = CreateCategory();
                var categoryList = new List<Category>() { category };

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == category.Name)).ReturnsAsync(categoryList);

                // Act
                var result = await _categoryService.Add(category);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallAddFromRepository_OnlyOnce()
            {
                // Arrange
                var category = CreateCategory();

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
                var category = CreateCategory();

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name == category.Name && c.Id != category.Id))
                    .ReturnsAsync(new List<Category>());
                _categoryRepositoryMock.Setup(c => c.Update(category));

                // Act
                var result = await _categoryService.Update(category);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<Category>();
            }

            [Fact]
            public async void ShouldNotUpdateCategory_WhenCategoryDoesNotExist()
            {
                // Arrange
                var category = CreateCategory();
                var categoryList = new List<Category>()
                {
                    new Category()
                    {
                        Id = 2,
                        Name = "Category Name 2"
                    }
                };

                _categoryRepositoryMock.Setup(c =>
                        c.Search(c => c.Name == category.Name && c.Id != category.Id))
                    .ReturnsAsync(categoryList);

                // Act
                var result = await _categoryService.Update(category);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallUpdateFromRepository_OnlyOnce()
            {
                // Arrange
                var category = CreateCategory();

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
                var category = CreateCategory();

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
                var category = CreateCategory();

                var books = new List<Book>()
                {
                    new Book()
                    {
                        Id = 1,
                        Name = "Test Name 1",
                        Author = "Test Author 1",
                        CategoryId = category.Id
                    }
                };

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
                var category = CreateCategory();

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
                var categoryList = CreateCategoryList();
                var searchedCategory = CreateCategory();
                var categoryName = searchedCategory.Name;

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name.Contains(categoryName)))
                    .ReturnsAsync(categoryList);

                // Act
                var result = await _categoryService.Search(searchedCategory.Name);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<List<Category>>();
            }

            [Fact]
            public async void ShouldReturnNull_WhenCategoriesWithSearchedNameDoNotExist()
            {
                // Arrange
                var searchedCategory = CreateCategory();
                var categoryName = searchedCategory.Name;

                _categoryRepositoryMock.Setup(c =>
                    c.Search(c => c.Name.Contains(categoryName)))
                    .ReturnsAsync((IEnumerable<Category>)(null));

                // Act
                var result = await _categoryService.Search(searchedCategory.Name);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async void ShouldCallSearchFromRepository_OnlyOnce()
            {
                // Arrange
                var categoryList = CreateCategoryList();
                var searchedCategory = CreateCategory();
                var categoryName = searchedCategory.Name;

                _categoryRepositoryMock.Setup(c =>
                        c.Search(c => c.Name.Contains(categoryName)))
                    .ReturnsAsync(categoryList);

                // Act
                await _categoryService.Search(searchedCategory.Name);

                // Assert
                _categoryRepositoryMock.Verify(mock => mock.Search(c => c.Name.Contains(categoryName)), Times.Once);
            }
        }
    }
}