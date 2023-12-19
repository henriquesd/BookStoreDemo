using AutoFixture;
using AutoMapper;
using BookStore.API.Controllers;
using BookStore.API.Dtos.Category;
using BookStore.API.Tests.Helpers;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using static BookStore.API.Dtos.PaginationDto;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.API.Tests
{
    public class CategoriesControllerTests
    {
        public abstract class CategoriesControllerTestsBase
        {
            protected readonly Fixture _fixture;
            protected readonly CategoriesController _categoriesController;
            protected readonly Mock<ICategoryService> _categoryServiceMock;
            protected readonly Mock<IMapper> _mapperMock;

            protected CategoriesControllerTestsBase()
            {
                _fixture = FixtureFactory.Create();
                _categoryServiceMock = new Mock<ICategoryService>();
                _mapperMock = new Mock<IMapper>();
                _categoriesController = new CategoriesController(_mapperMock.Object, _categoryServiceMock.Object);
            }
        }

        public class GetAll : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenExistCategory()
            {
                // Arrange
                var categories = _fixture.Build<Category>()
                    .CreateMany();
                var categoryListResultDto = _fixture.CreateMany<CategoryResultDto>();

                _categoryServiceMock.Setup(c => c.GetAll()).ReturnsAsync(categories);
                _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResultDto>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryListResultDto);

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
                var categoryListResultDto = _fixture.CreateMany<CategoryResultDto>();

                _categoryServiceMock.Setup(c => c.GetAll()).ReturnsAsync(categories);
                _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResultDto>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryListResultDto);

                // Act
                var result = await _categoriesController.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldCallGetAllFromService_OnlyOnce()
            {
                // Arrange
                var categories = _fixture.Build<Category>()
                   .CreateMany();
                var categoryListResultDto = _fixture.CreateMany<CategoryResultDto>();

                _categoryServiceMock.Setup(c => c.GetAll()).ReturnsAsync(categories);
                _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResultDto>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryListResultDto);

                // Act
                await _categoriesController.GetAll();

                // Assert
                _categoryServiceMock.Verify(mock => mock.GetAll(), Times.Once);
            }
        }

        public class GetAllWithPagination : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenExistCategory()
            {
                // Arrange
                var categories = _fixture.Build<Category>()
                    .CreateMany()
                    .ToList();
                var pagedResponse = _fixture.Build<PagedResponse<Category>>()
                    .With(p => p.Data, categories)
                    .Create();
                var pagedResponseDto = _fixture.Build<PagedResponseDto<CategoryResultDto>>()
                    .With(p => p.Data, _fixture.Build<CategoryResultDto>().CreateMany().ToList())
                    .Create();

                _categoryServiceMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);
                _mapperMock.Setup(m => m.Map<PagedResponseDto<CategoryResultDto>>(It.IsAny<PagedResponse<Category>>())).Returns(pagedResponseDto);

                // Act
                var result = await _categoriesController.GetAllWithPagination();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldReturnOk_WhenDoesNotExistAnyCategory()
            {
                // Arrange
                var pagedResponse = _fixture.Build<PagedResponse<Category>>()
                    .Without(p => p.Data)
                    .Create();
                var pagedResponseDto = _fixture.Build<PagedResponseDto<CategoryResultDto>>()
                    .Without(p => p.Data)
                    .Create();

                _categoryServiceMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);
                _mapperMock.Setup(m => m.Map<PagedResponseDto<CategoryResultDto>>(It.IsAny<PagedResponse<Category>>())).Returns(pagedResponseDto);

                // Act
                var result = await _categoriesController.GetAllWithPagination();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldCallGetAllWithPaginationFromService_OnlyOnce()
            {
                // Arrange
                var categories = _fixture.Build<Category>()
                    .CreateMany()
                    .ToList();
                var pagedResponse = _fixture.Build<PagedResponse<Category>>()
                    .With(p => p.Data, categories)
                    .Create();
                var pagedResponseDto = _fixture.Build<PagedResponseDto<CategoryResultDto>>()
                    .With(p => p.Data, _fixture.Build<CategoryResultDto>().CreateMany().ToList())
                    .Create();

                _categoryServiceMock.Setup(c => c.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);
                _mapperMock.Setup(m => m.Map<PagedResponseDto<CategoryResultDto>>(It.IsAny<PagedResponse<Category>>())).Returns(pagedResponseDto);

                // Act
                await _categoriesController.GetAllWithPagination();

                // Assert
                _categoryServiceMock.Verify(mock => mock.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            }

            [Theory]
            [InlineData(0, 10)]
            [InlineData(1, 0)]
            public async void ShouldReturnBadRequest_WhenPaginationParametersAreInvalid(int pageNumber, int pageSize)
            {
                // Act
                var result = await _categoriesController.GetAllWithPagination(pageNumber, pageSize);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }
        }

        public class GetById : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenCategoryExist()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var categoryResultDto = _fixture.Create<CategoryResultDto>();

                _mapperMock.Setup(m => m.Map<CategoryResultDto>(It.IsAny<Category>())).Returns(categoryResultDto);
                _categoryServiceMock.Setup(c => c.GetById(2)).ReturnsAsync(category);

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
                var category = _fixture.Create<Category>();
                var categoryResultDto = _fixture.Create<CategoryResultDto>();

                _categoryServiceMock.Setup(c => c.GetById(2)).ReturnsAsync(category);
                _mapperMock.Setup(m => m.Map<CategoryResultDto>(It.IsAny<Category>())).Returns(categoryResultDto);

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
                var category = _fixture.Create<Category>();
                var categoryResult = _fixture.Build<OperationResult<Category>>()
                    .With(p => p.Payload, category)
                    .Create();
                var categoryAddDto = _fixture.Build<CategoryAddDto>()
                    .With(p => p.Name, category.Name)
                    .Create();
                var categoryResultDto = _fixture.Create<CategoryResultDto>();

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryAddDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.Add(category)).ReturnsAsync(categoryResult);
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
            public async void ShouldCallAddFromService_OnlyOnce()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var categoryResult = _fixture.Build<OperationResult<Category>>()
                    .With(p => p.Payload, category)
                    .Create();
                var categoryAddDto = _fixture.Build<CategoryAddDto>()
                   .With(p => p.Name, category.Name)
                   .Create();

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryAddDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.Add(category)).ReturnsAsync(categoryResult);

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
                var category = _fixture.Create<Category>();
                var categoryResult = _fixture.Build<OperationResult<Category>>()
                    .With(p => p.Payload, category)
                    .Create();
                var categoryEditDto = _fixture.Build<CategoryEditDto>()
                    .With(p => p.Id, category.Id)
                    .With(p => p.Name, "NameUpdated")
                    .Create();

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryEditDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync(category);
                _categoryServiceMock.Setup(c => c.Update(category)).ReturnsAsync(categoryResult);

                // Act
                var result = await _categoriesController.Update(categoryEditDto.Id, categoryEditDto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async void ShouldCallUpdateFromService_OnlyOnce()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var categoryResult = _fixture.Build<OperationResult<Category>>()
                    .With(p => p.Payload, category)
                    .Create();
                var categoryEditDto = _fixture.Build<CategoryEditDto>()
                   .With(p => p.Id, category.Id)
                   .With(p => p.Name, "NameUpdated")
                   .Create();

                _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CategoryEditDto>())).Returns(category);
                _categoryServiceMock.Setup(c => c.GetById(category.Id)).ReturnsAsync(category);
                _categoryServiceMock.Setup(c => c.Update(category)).ReturnsAsync(categoryResult);

                // Act
                await _categoriesController.Update(categoryEditDto.Id, categoryEditDto);

                // Assert
                _categoryServiceMock.Verify(mock => mock.Update(category), Times.Once);
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
        }

        public class Remove : CategoriesControllerTestsBase
        {
            [Fact]
            public async void ShouldReturnOk_WhenCategoryIsRemoved()
            {
                // Arrange
                var category = _fixture.Create<Category>();

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
                var category = _fixture.Create<Category>();

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
                var category = _fixture.Create<Category>();

                _categoryServiceMock.Setup(c => c.GetById(category.Id)                ).ReturnsAsync(category);
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
                var category = _fixture.Create<Category>();

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
                var categoryList = _fixture.CreateMany<Category>();
                var category = _fixture.Create<Category>();

                _categoryServiceMock.Setup(c => c.Search(category.Name))
                    .ReturnsAsync(categoryList);
                _mapperMock.Setup(m => m.Map<List<Category>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryList.ToList());

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
                var categoryName = _fixture.Create<string>();

                _categoryServiceMock.Setup(c => c.Search(categoryName)).ReturnsAsync(new List<Category>());

                // Act
                var result = await _categoriesController.Search(categoryName);
                var actual = (NotFoundObjectResult)result.Result;

                // Assert
                actual.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async void ShouldCallSearchFromService_OnlyOnce()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var categoryList = _fixture.CreateMany<Category>();

                _categoryServiceMock.Setup(c => c.Search(category.Name)).ReturnsAsync(categoryList);
                _mapperMock.Setup(m => m.Map<List<Category>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryList.ToList());

                // Act
                await _categoriesController.Search(category.Name);

                // Assert
                _categoryServiceMock.Verify(mock => mock.Search(category.Name), Times.Once);
            }
        }
    }
}