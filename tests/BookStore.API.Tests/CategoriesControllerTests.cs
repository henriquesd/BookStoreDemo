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
using NSubstitute;
using Xunit;

namespace BookStore.API.Tests
{
    public class CategoriesControllerTests
    {
        private readonly Fixture _fixture;
        private readonly ICategoryService _categoryServiceMock;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _fixture = FixtureFactory.Create();
            _categoryServiceMock = Substitute.For<ICategoryService>();
            _controller = new CategoriesController(_categoryServiceMock);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = _fixture.CreateMany<Category>(3).ToList();
            _categoryServiceMock.GetAll().Returns(OperationResult<IEnumerable<Category>>.Ok(categories));

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedCategories = okResult!.Value as IEnumerable<CategoryResultDto>;
            returnedCategories.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            _categoryServiceMock.GetAll().Returns(OperationResult<IEnumerable<Category>>.Ok(new List<Category>()));

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedCategories = okResult!.Value as IEnumerable<CategoryResultDto>;
            returnedCategories.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            _categoryServiceMock.GetAll().Returns(OperationResult<IEnumerable<Category>>.Ok(new List<Category>()));

            // Act
            await _controller.GetAll();

            // Assert
            await _categoryServiceMock.Received(1).GetAll();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnOkWithPagedCategories_WhenCategoriesExist()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var categories = _fixture.CreateMany<Category>(5).ToList();
            var pagedResponse = new PagedResponse<Category>(categories, pageNumber, pageSize, categories.Count);
            _categoryServiceMock.GetAllWithPagination(pageNumber, pageSize).Returns(OperationResult<PagedResponse<Category>>.Ok(pagedResponse));

            // Act
            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var pagedDto = (PagedResponseDto<CategoryResultDto>)okResult!.Value!;
            pagedDto.Data.Should().HaveCount(categories.Count);
            pagedDto.PageNumber.Should().Be(pageNumber);
            pagedDto.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnOkWithEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var pagedResponse = new PagedResponse<Category>(new List<Category>(), 1, 10, 0);
            _categoryServiceMock.GetAllWithPagination(1, 10).Returns(OperationResult<PagedResponse<Category>>.Ok(pagedResponse));

            // Act
            var result = await _controller.GetAllWithPagination();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(-5, -5)]
        [InlineData(1, 101)] // pageSize > 100
        public async Task GetAllWithPagination_ShouldReturnBadRequest_WhenParametersAreInvalid(int pageNumber, int pageSize)
        {
            // Arrange
            var validationError = OperationResult<PagedResponse<Category>>.ValidationError("Page number and page size must be valid");
            _categoryServiceMock.GetAllWithPagination(pageNumber, pageSize).Returns(validationError);

            // Act
            var result = await _controller.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var pagedResponse = new PagedResponse<Category>(new List<Category>(), pageNumber, pageSize, 0);
            _categoryServiceMock.GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>()).Returns(OperationResult<PagedResponse<Category>>.Ok(pagedResponse));

            // Act
            await _controller.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            await _categoryServiceMock.Received(1).GetAllWithPagination(pageNumber, pageSize);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithCategory_WhenCategoryExists()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryServiceMock.GetById(category.Id).Returns(OperationResult<Category>.Ok(category));

            // Act
            var result = await _controller.GetById(category.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult!.Value as CategoryResultDto;
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(category.Id);
            dto.Name.Should().Be(category.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            _categoryServiceMock.GetById(Arg.Any<int>()).Returns(OperationResult<Category>.NotFound($"Category with ID {categoryId} not found"));

            // Act
            var result = await _controller.GetById(categoryId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryServiceMock.GetById(category.Id).Returns(OperationResult<Category>.Ok(category));

            // Act
            await _controller.GetById(category.Id);

            // Assert
            await _categoryServiceMock.Received(1).GetById(category.Id);
        }

        [Fact]
        public async Task Add_ShouldReturnCreatedWithCategory_WhenCategoryIsValid()
        {
            // Arrange
            var dto = _fixture.Create<CategoryAddDto>();
            var categoryFromDto = dto.ToModel();
            var category = new Category
            {
                Id = _fixture.Create<int>(),
                Name = categoryFromDto.Name
            };
            var operationResult = OperationResult<Category>.SuccessResult(category);
            _categoryServiceMock.Add(Arg.Any<Category>()).Returns(operationResult);

            // Act
            var result = await _controller.Add(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var resultDto = createdResult!.Value as CategoryResultDto;
            resultDto.Should().NotBeNull();
            resultDto!.Id.Should().Be(category.Id);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenServiceReturnsFails()
        {
            // Arrange
            var dto = _fixture.Create<CategoryAddDto>();
            var errorMessage = _fixture.Create<string>();
            var operationResult = OperationResult<Category>.ValidationError(errorMessage);
            _categoryServiceMock.Add(Arg.Any<Category>()).Returns(operationResult);

            // Act
            var result = await _controller.Add(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Add_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var dto = _fixture.Create<CategoryAddDto>();
            var category = dto.ToModel();
            var operationResult = OperationResult<Category>.SuccessResult(category);
            _categoryServiceMock.Add(Arg.Any<Category>()).Returns(operationResult);

            // Act
            await _controller.Add(dto);

            // Assert
            await _categoryServiceMock.Received(1).Add(Arg.Any<Category>());
        }

        [Fact]
        public async Task Update_ShouldReturnOkWithCategory_WhenCategoryIsValid()
        {
            // Arrange
            var dto = _fixture.Create<CategoryEditDto>();
            var category = dto.ToModel();
            var operationResult = OperationResult<Category>.SuccessResult(category);
            _categoryServiceMock.Update(Arg.Any<Category>()).Returns(operationResult);

            // Act
            var result = await _controller.Update(dto.Id, dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultDto = okResult!.Value as CategoryResultDto;
            resultDto.Should().NotBeNull();
            resultDto!.Id.Should().Be(dto.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var dto = _fixture.Create<CategoryEditDto>();
            var differentUrlId = dto.Id + _fixture.Create<int>() + 1;

            // Act
            var result = await _controller.Update(differentUrlId, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenServiceReturnsNotFound()
        {
            // Arrange
            var dto = _fixture.Create<CategoryEditDto>();
            var operationResult = OperationResult<Category>.NotFound(_fixture.Create<string>());
            _categoryServiceMock.Update(Arg.Any<Category>(), Arg.Any<CancellationToken>()).Returns(operationResult);

            // Act
            var result = await _controller.Update(dto.Id, dto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var dto = _fixture.Create<CategoryEditDto>();
            var category = dto.ToModel();
            var operationResult = OperationResult<Category>.SuccessResult(category);
            _categoryServiceMock.Update(Arg.Any<Category>()).Returns(operationResult);

            // Act
            await _controller.Update(dto.Id, dto);

            // Assert
            await _categoryServiceMock.Received(1).Update(Arg.Any<Category>());
        }

        [Fact]
        public async Task Remove_ShouldReturnNoContent_WhenCategoryIsRemoved()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.SuccessResult(true);
            _categoryServiceMock.Remove(categoryId).Returns(operationResult);

            // Act
            var result = await _controller.Remove(categoryId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.NotFound(_fixture.Create<string>());
            _categoryServiceMock.Remove(categoryId, Arg.Any<CancellationToken>()).Returns(operationResult);

            // Act
            var result = await _controller.Remove(categoryId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Remove_ShouldReturnBadRequest_WhenCategoryHasAssociatedBooks()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.ValidationError(_fixture.Create<string>());
            _categoryServiceMock.Remove(categoryId).Returns(operationResult);

            // Act
            var result = await _controller.Remove(categoryId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Remove_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var operationResult = OperationResult<bool>.SuccessResult(true);
            _categoryServiceMock.Remove(categoryId).Returns(operationResult);

            // Act
            await _controller.Remove(categoryId);

            // Assert
            await _categoryServiceMock.Received(1).Remove(categoryId);
        }

        [Fact]
        public async Task Search_ShouldReturnOkWithCategories_WhenCategoriesFound()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var categories = _fixture.CreateMany<Category>(2).ToList();
            _categoryServiceMock.Search(searchTerm).Returns(OperationResult<IEnumerable<Category>>.Ok(categories));

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<CategoryResultDto>;
            dtos.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_ShouldReturnOkWithEmptyList_WhenNoCategoriesFound()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            _categoryServiceMock.Search(searchTerm).Returns(OperationResult<IEnumerable<Category>>.Ok(new List<Category>()));

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dtos = okResult!.Value as IEnumerable<CategoryResultDto>;
            dtos.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Search_ShouldReturnBadRequest_WhenSearchTermIsEmpty(string? searchTerm)
        {
            // Arrange
            var validationError = OperationResult<IEnumerable<Category>>.ValidationError("Search term is required");
            _categoryServiceMock.Search(searchTerm!).Returns(validationError);

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Search_ShouldCallServiceOnce_WhenCalled()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var categories = _fixture.CreateMany<Category>(1).ToList();
            _categoryServiceMock.Search(searchTerm).Returns(OperationResult<IEnumerable<Category>>.Ok(categories));

            // Act
            await _controller.Search(searchTerm);

            // Assert
            await _categoryServiceMock.Received(1).Search(searchTerm);
        }
    }
}
