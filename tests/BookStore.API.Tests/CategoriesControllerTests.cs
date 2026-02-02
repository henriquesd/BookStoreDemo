using AutoFixture;
using BookStore.API.Controllers;
using BookStore.API.Dtos;
using BookStore.API.Dtos.Category;
using BookStore.API.Mappings;
using BookStore.API.Tests.Helpers;
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
        private readonly Fixture _fixture;
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _fixture = FixtureFactory.Create();
            _categoryServiceMock = new Mock<ICategoryService>();
            _controller = new CategoriesController(_categoryServiceMock.Object);
        }

        private Category CreateCategory(int id = 1, string name = "Test Category")
        {
            return new Category
            {
                Id = id,
                Name = name
            };
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithCategories_WhenCategoriesExist()
        {
            var categories = new List<Category>
            {
                CreateCategory(1, "Category 1"),
                CreateCategory(2, "Category 2"),
                CreateCategory(3, "Category 3")
            };

            _categoryServiceMock
                .Setup(s => s.GetAll())
                .ReturnsAsync(categories);

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedCategories = okResult.Value as IEnumerable<CategoryResultDto>;
            returnedCategories.Should().HaveCount(3);
            returnedCategories.First().Name.Should().Be("Category 1");
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoCategoriesExist()
        {
            _categoryServiceMock
                .Setup(s => s.GetAll())
                .ReturnsAsync(new List<Category>());

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedCategories = okResult.Value as IEnumerable<CategoryResultDto>;
            returnedCategories.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ShouldCallServiceOnce_WhenCalled()
        {
            _categoryServiceMock
                .Setup(s => s.GetAll())
                .ReturnsAsync(new List<Category>());

            await _controller.GetAll();

            _categoryServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        [Theory]
        [InlineData(1, 10, 3)]
        [InlineData(2, 5, 10)]
        [InlineData(1, 20, 1)]
        public async Task GetAllWithPagination_ShouldReturnOkWithPagedCategories_WhenCategoriesExist(int pageNumber, int pageSize, int totalRecords)
        {
            var categories = Enumerable.Range(1, totalRecords)
                .Select(i => CreateCategory(i, $"Category {i}"))
                .ToList();
            var pagedResponse = new PagedResponse<Category>(categories, pageNumber, pageSize, totalRecords);
            _categoryServiceMock.Setup(s => s.GetAllWithPagination(pageNumber, pageSize)).ReturnsAsync(pagedResponse);

            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var pagedDto = (PagedResponseDto<CategoryResultDto>)okResult.Value;
            pagedDto.Data.Should().HaveCount(totalRecords);
            pagedDto.PageNumber.Should().Be(pageNumber);
            pagedDto.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnOkWithEmptyList_WhenNoCategoriesExist()
        {
            var pagedResponse = new PagedResponse<Category>(new List<Category>(), 1, 10, 0);
            _categoryServiceMock.Setup(s => s.GetAllWithPagination(1, 10)).ReturnsAsync(pagedResponse);

            var result = await _controller.GetAllWithPagination();

            result.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(-5, -5)]
        public async Task GetAllWithPagination_ShouldReturnBadRequest_WhenParametersAreInvalid(int pageNumber, int pageSize)
        {
            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallServiceOnce_WhenCalled()
        {
            var pagedResponse = new PagedResponse<Category>(new List<Category>(), 1, 10, 0);
            _categoryServiceMock.Setup(s => s.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

            await _controller.GetAllWithPagination(1, 10);

            _categoryServiceMock.Verify(s => s.GetAllWithPagination(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithCategory_WhenCategoryExists()
        {
            var category = CreateCategory(1, "Test Category");
            _categoryServiceMock.Setup(s => s.GetById(1)).ReturnsAsync(category);

            var result = await _controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult.Value as CategoryResultDto;
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Test Category");
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            _categoryServiceMock.Setup(s => s.GetById(It.IsAny<int>())).ReturnsAsync((Category)null);

            var result = await _controller.GetById(999);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetById_ShouldCallServiceOnce_WhenCalled(int categoryId)
        {
            var category = CreateCategory(categoryId);
            _categoryServiceMock.Setup(s => s.GetById(categoryId)).ReturnsAsync(category);

            await _controller.GetById(categoryId);

            _categoryServiceMock.Verify(s => s.GetById(categoryId), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldReturnCreatedWithCategory_WhenCategoryIsValid()
        {
            var dto = _fixture.Create<CategoryAddDto>();
            var category = dto.ToModel();
            category.Id = 1; // Set ID to simulate saved entity
            var operationResult = new OperationResult<Category>(category, true, null);
            _categoryServiceMock.Setup(s => s.Add(It.IsAny<Category>())).ReturnsAsync(operationResult);

            var result = await _controller.Add(dto);

            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var resultDto = createdResult.Value as CategoryResultDto;
            resultDto.Should().NotBeNull();
            resultDto.Id.Should().Be(1);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenServiceReturnsFails()
        {
            var dto = _fixture.Create<CategoryAddDto>();
            var operationResult = new OperationResult<Category>(false, "Category already exists");
            _categoryServiceMock.Setup(s => s.Add(It.IsAny<Category>())).ReturnsAsync(operationResult);

            var result = await _controller.Add(dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Add_ShouldCallServiceOnce_WhenCalled()
        {
            var dto = _fixture.Create<CategoryAddDto>();
            var category = dto.ToModel();
            var operationResult = new OperationResult<Category>(category, true, null);
            _categoryServiceMock.Setup(s => s.Add(It.IsAny<Category>())).ReturnsAsync(operationResult);

            await _controller.Add(dto);

            _categoryServiceMock.Verify(s => s.Add(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnOkWithCategory_WhenCategoryIsValid()
        {
            var dto = _fixture.Create<CategoryEditDto>();
            var category = dto.ToModel();
            var operationResult = new OperationResult<Category>(category, true, null);
            _categoryServiceMock.Setup(s => s.Update(It.IsAny<Category>())).ReturnsAsync(operationResult);

            var result = await _controller.Update(dto.Id, dto);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultDto = okResult.Value as CategoryResultDto;
            resultDto.Should().NotBeNull();
            resultDto.Id.Should().Be(dto.Id);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 10)]
        [InlineData(100, 999)]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch(int urlId, int dtoId)
        {
            var dto = _fixture.Build<CategoryEditDto>().With(c => c.Id, dtoId).Create();

            var result = await _controller.Update(urlId, dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenServiceReturnsNotFound()
        {
            var dto = _fixture.Create<CategoryEditDto>();
            var operationResult = new OperationResult<Category>(false, "Category not found");
            _categoryServiceMock.Setup(s => s.Update(It.IsAny<Category>())).ReturnsAsync(operationResult);

            var result = await _controller.Update(dto.Id, dto);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldCallServiceOnce_WhenCalled()
        {
            var dto = _fixture.Create<CategoryEditDto>();
            var category = dto.ToModel();
            var operationResult = new OperationResult<Category>(category, true, null);
            _categoryServiceMock.Setup(s => s.Update(It.IsAny<Category>())).ReturnsAsync(operationResult);

            await _controller.Update(dto.Id, dto);

            _categoryServiceMock.Verify(s => s.Update(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task Remove_ShouldReturnNoContent_WhenCategoryIsRemoved()
        {
            var operationResult = new OperationResult<bool>(true, true, null);
            _categoryServiceMock.Setup(s => s.Remove(1)).ReturnsAsync(operationResult);

            var result = await _controller.Remove(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            var operationResult = new OperationResult<bool>(false, "Category with ID 1 not found");
            _categoryServiceMock.Setup(s => s.Remove(1)).ReturnsAsync(operationResult);

            var result = await _controller.Remove(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnBadRequest_WhenCategoryHasAssociatedBooks()
        {
            var operationResult = new OperationResult<bool>(false, "Cannot delete category with associated books");
            _categoryServiceMock.Setup(s => s.Remove(1)).ReturnsAsync(operationResult);

            var result = await _controller.Remove(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(999)]
        public async Task Remove_ShouldCallServiceOnce_WhenCalled(int categoryId)
        {
            var operationResult = new OperationResult<bool>(true, true, null);
            _categoryServiceMock.Setup(s => s.Remove(categoryId)).ReturnsAsync(operationResult);

            await _controller.Remove(categoryId);

            _categoryServiceMock.Verify(s => s.Remove(categoryId), Times.Once);
        }

        [Theory]
        [InlineData("Fiction")]
        [InlineData("Science")]
        [InlineData("Biography")]
        public async Task Search_ShouldReturnOkWithCategories_WhenCategoriesFound(string searchTerm)
        {
            var categories = new List<Category> { CreateCategory(1, searchTerm) };
            _categoryServiceMock.Setup(s => s.Search(searchTerm)).ReturnsAsync(categories);

            var result = await _controller.Search(searchTerm);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult.Value as IEnumerable<CategoryResultDto>;
            dtos.Should().HaveCount(1);
            dtos.First().Name.Should().Be(searchTerm);
        }

        [Fact]
        public async Task Search_ShouldReturnNotFound_WhenNoCategoriesFound()
        {
            _categoryServiceMock.Setup(s => s.Search("NonExistent")).ReturnsAsync(new List<Category>());

            var result = await _controller.Search("NonExistent");

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Search_ShouldReturnBadRequest_WhenSearchTermIsEmpty(string searchTerm)
        {
            var result = await _controller.Search(searchTerm);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Search_ShouldCallServiceOnce_WhenCalled()
        {
            var categories = new List<Category> { CreateCategory() };
            _categoryServiceMock.Setup(s => s.Search("Test")).ReturnsAsync(categories);

            await _controller.Search("Test");

            _categoryServiceMock.Verify(s => s.Search("Test"), Times.Once);
        }
    }
}
