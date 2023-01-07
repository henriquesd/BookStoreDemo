using AutoMapper;
using BookStore.API.Controllers;
using BookStore.API.Dtos.Category;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookStore.API.Tests
{
    public class CategoriesControllerTests
    {
        public abstract class CategoriesControllerTestsBase
        {
            protected readonly CategoriesController _categoriesController;
            protected readonly Mock<ICategoryService> _categoryServiceMock;
            protected readonly Mock<IMapper> _mapperMock;

            protected CategoriesControllerTestsBase()
            {
                _categoryServiceMock = new Mock<ICategoryService>();
                _mapperMock = new Mock<IMapper>();
                _categoriesController = new CategoriesController(_mapperMock.Object, _categoryServiceMock.Object);
            }

            protected Category CreateCategory()
            {
                return new Category()
                {
                    Id = 2,
                    Name = "Category Name 2"
                };
            }

            protected CategoryResultDto MapModelToCategoryResultDto(Category category)
            {
                var categoryDto = new CategoryResultDto()
                {
                    Id = category.Id,
                    Name = category.Name
                };
                return categoryDto;
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

            protected List<CategoryResultDto> MapModelToCategoryListDto(List<Category> categories)
            {
                var listCategories = new List<CategoryResultDto>();

                foreach (var item in categories)
                {
                    var category = new CategoryResultDto()
                    {
                        Id = item.Id,
                        Name = item.Name
                    };
                    listCategories.Add(category);
                }
                return listCategories;
            }
        }

        public class GetAll : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenExistCategory()
            {
                // Arrange
                var categories = CreateCategoryList();
                var dtoExpected = MapModelToCategoryListDto(categories);

                _categoryServiceMock.Setup(c => c.GetAll()).ReturnsAsync(categories);
                _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResultDto>>(
                    It.IsAny<List<Category>>())).Returns(dtoExpected);

                // Act
                var result = await _categoriesController.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnOk_WhenDoesNotExistAnyCategory()
            {
                // Arrange
                var categories = new List<Category>();
                var dtoExpected = MapModelToCategoryListDto(categories);

                _categoryServiceMock.Setup(c => c.GetAll()).ReturnsAsync(categories);
                _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResultDto>>(It.IsAny<List<Category>>())).Returns(dtoExpected);

                // Act
                var result = await _categoriesController.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldCallGetAllFromService_OnlyOnce()
            {
                // Arrange
                var categories = CreateCategoryList();
                var dtoExpected = MapModelToCategoryListDto(categories);

                _categoryServiceMock.Setup(c => c.GetAll()).ReturnsAsync(categories);
                _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResultDto>>(It.IsAny<List<Category>>())).Returns(dtoExpected);

                // Act
                await _categoriesController.GetAll();

                // Assert
                _categoryServiceMock.Verify(mock => mock.GetAll(), Times.Once);
            }
        }

        public class GetById : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenCategoryExist()
            {
                // Arrange
                var category = CreateCategory();
                var dtoExpected = MapModelToCategoryResultDto(category);

                _categoryServiceMock.Setup(c => c.GetById(2)).ReturnsAsync(category);
                _mapperMock.Setup(m => m.Map<CategoryResultDto>(It.IsAny<Category>())).Returns(dtoExpected);

                // Act
                var result = await _categoriesController.GetById(2);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenCategoryDoesNotExist()
            {
                // Arrange
                _categoryServiceMock.Setup(c => c.GetById(2)).ReturnsAsync((Category)null);

                // Act
                var result = await _categoriesController.GetById(2);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async void ShouldCallGetByIdFromService_OnlyOnce()
            {
                // Arrange
                var category = CreateCategory();
                var dtoExpected = MapModelToCategoryResultDto(category);

                _categoryServiceMock.Setup(c => c.GetById(2)).ReturnsAsync(category);
                _mapperMock.Setup(m => m.Map<CategoryResultDto>(It.IsAny<Category>())).Returns(dtoExpected);

                // Act
                await _categoriesController.GetById(2);

                // Assert
                _categoryServiceMock.Verify(mock => mock.GetById(2), Times.Once);
            }
        }

        public class Add : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenCategoryIsAdded()
            {
                // Arrange
                var category = CreateCategory();
                var categoryAddDto = new CategoryAddDto() { Name = category.Name };
                var categoryResultDto = MapModelToCategoryResultDto(category);

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryAddDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.Add(category)).ReturnsAsync(category);
                _mapperMock.Setup(m => m.Map<CategoryResultDto>(It.IsAny<Category>())).Returns(categoryResultDto);

                // Act
                var result = await _categoriesController.Add(categoryAddDto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenModelStateIsInvalid()
            {
                // Arrange
                var categoryAddDto = new CategoryAddDto();
                _categoriesController.ModelState.AddModelError("Name", "The field name is required");

                // Act
                var result = await _categoriesController.Add(categoryAddDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenCategoryResultIsNull()
            {
                // Arrange
                var category = CreateCategory();
                var categoryAddDto = new CategoryAddDto() { Name = category.Name };

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryAddDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.Add(category)).ReturnsAsync((Category)null);

                // Act
                var result = await _categoriesController.Add(categoryAddDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldCallAddFromService_OnlyOnce()
            {
                // Arrange
                var category = CreateCategory();
                var categoryAddDto = new CategoryAddDto() { Name = category.Name };

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryAddDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.Add(category)).ReturnsAsync(category);

                // Act
                await _categoriesController.Add(categoryAddDto);

                // Assert
                _categoryServiceMock.Verify(mock => mock.Add(category), Times.Once);
            }
        }

        public class Update : CategoriesControllerTestsBase
        {

            [Fact]
            public async void ShouldReturnOk_WhenCategoryIsUpdatedCorrectly()
            {
                // Arrange
                var category = CreateCategory();
                var categoryEditDto = new CategoryEditDto() { Id = category.Id, Name = "Test" };

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryEditDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync(category);
                _categoryServiceMock.Setup(c => c.Update(category)).ReturnsAsync(category);

                // Act
                var result = await _categoriesController.Update(categoryEditDto.Id, categoryEditDto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenCategoryIdIsDifferentThenParameterId()
            {
                // Arrange
                var categoryEditDto = new CategoryEditDto() { Id = 1, Name = "Test" };

                // Act
                var result = await _categoriesController.Update(2, categoryEditDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenModelStateIsInvalid()
            {
                // Arrange
                var categoryEditDto = new CategoryEditDto() { Id = 1 };
                _categoriesController.ModelState.AddModelError("Name", "The field name is required");

                // Act
                var result = await _categoriesController.Update(1, categoryEditDto);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldCallUpdateFromService_OnlyOnce()
            {
                // Arrange
                var category = CreateCategory();
                var categoryEditDto = new CategoryEditDto() { Id = category.Id, Name = "Test" };

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryEditDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync(category);
                _categoryServiceMock.Setup(c => c.Update(category)).ReturnsAsync(category);

                // Act
                await _categoriesController.Update(categoryEditDto.Id, categoryEditDto);

                // Assert
                _categoryServiceMock.Verify(mock => mock.Update(category), Times.Once);
            }
        }

        public class Remove : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenCategoryIsRemoved()
            {
                // Arrange
                var category = CreateCategory();
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync(category);
                _categoryServiceMock.Setup(c => c.Remove(category)).ReturnsAsync(true);

                // Act
                var result = await _categoriesController.Remove(category.Id);

                // Assert
                result.Should().BeOfType<OkResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenCategoryDoesNotExist()
            {
                // Arrange
                var category = CreateCategory();
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync((Category)null);

                // Act
                var result = await _categoriesController.Remove(category.Id);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async void ShouldReturnBadRequest_WhenResultIsFalse()
            {
                // Arrange
                var category = CreateCategory();
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync(category);
                _categoryServiceMock.Setup(c => c.Remove(category)).ReturnsAsync(false);

                // Act
                var result = await _categoriesController.Remove(category.Id);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async void ShouldCallRemoveFromService_OnlyOnce()
            {
                // Arrange
                var category = CreateCategory();
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync(category);
                _categoryServiceMock.Setup(c => c.Remove(category)).ReturnsAsync(true);

                // Act
                await _categoriesController.Remove(category.Id);

                // Assert
                _categoryServiceMock.Verify(mock => mock.Remove(category), Times.Once);
            }
        }

        public class Search : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenCategoryWithSearchedNameExist()
            {
                // Arrange
                var categoryList = CreateCategoryList();
                var category = CreateCategory();

                _categoryServiceMock.Setup(c => c.Search(category.Name))
                    .ReturnsAsync(categoryList);
                _mapperMock.Setup(m => m.Map<List<Category>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryList);

                // Act
                var result = await _categoriesController.Search(category.Name);
                var actual = (OkObjectResult)result.Result;

                // Assert
                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnNotFound_WhenCategoryWithSearchedNameDoesNotExist()
            {
                // Arrange
                var category = CreateCategory();
                var categoryList = new List<Category>();

                var dtoExpected = MapModelToCategoryResultDto(category);
                category.Name = dtoExpected.Name;

                _categoryServiceMock.Setup(c => c.Search(category.Name))
                    .ReturnsAsync(categoryList);
                _mapperMock.Setup(m => m.Map<IEnumerable<Category>>(It.IsAny<Category>())).Returns(categoryList);

                // Act
                var result = await _categoriesController.Search(category.Name);
                var actual = (NotFoundObjectResult)result.Result;

                // Assert
                actual.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async void ShouldCallSearchFromService_OnlyOnce()
            {
                // Arrange
                var categoryList = CreateCategoryList();
                var category = CreateCategory();

                _categoryServiceMock.Setup(c => c.Search(category.Name))
                    .ReturnsAsync(categoryList);
                _mapperMock.Setup(m => m.Map<List<Category>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryList);

                // Act
                await _categoriesController.Search(category.Name);

                // Assert
                _categoryServiceMock.Verify(mock => mock.Search(category.Name), Times.Once);
            }
        }
    }
}