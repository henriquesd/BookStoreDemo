using System.Linq.Expressions;
using AutoFixture;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Domain.Services;
using BookStore.Domain.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BookStore.Domain.Tests
{
    public class CategoryServiceTests
    {
        private readonly Fixture _fixture;
        private readonly ICategoryRepository _categoryRepositoryMock;
        private readonly IBookRepository _bookRepositoryMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _fixture = FixtureFactory.Create();
            _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
            _bookRepositoryMock = Substitute.For<IBookRepository>();
            _service = new CategoryService(_categoryRepositoryMock, _bookRepositoryMock);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = _fixture.CreateMany<Category>(3).ToList();
            _categoryRepositoryMock.GetAll().Returns(categories);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Category>>();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNull_WhenCategoriesDoNotExist()
        {
            // Arrange
            _categoryRepositoryMock.GetAll().Returns((List<Category>?)null);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            _categoryRepositoryMock.GetAll().Returns((List<Category>?)null);

            // Act
            await _service.GetAll();

            // Assert
            await _categoryRepositoryMock.Received(1).GetAll();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnPagedResponse_WhenCategoriesExist()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var categories = _fixture.CreateMany<Category>(5).ToList();
            var pagedResponse = new PagedResponse<Category>(categories, pageNumber, pageSize, categories.Count);
            _categoryRepositoryMock.GetAllWithPagination(pageNumber, pageSize).Returns(pagedResponse);

            // Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PagedResponse<Category>>();
            result.Data.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnEmptyList_WhenCategoriesDoNotExist()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var pagedResponse = new PagedResponse<Category>(new List<Category>(), pageNumber, pageSize, 0);
            _categoryRepositoryMock.GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>()).Returns(pagedResponse);

            // Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var pageNumber = _fixture.Create<int>() % 10 + 1;
            var pageSize = _fixture.Create<int>() % 50 + 1;
            var pagedResponse = _fixture.Create<PagedResponse<Category>>();
            _categoryRepositoryMock.GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>()).Returns(pagedResponse);

            // Act
            await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            await _categoryRepositoryMock.Received(1).GetAllWithPagination(Arg.Any<int>(), Arg.Any<int>());
        }

        [Fact]
        public async Task GetById_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.GetById(category.Id).Returns(category);

            // Act
            var result = await _service.GetById(category.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Category>();
            result!.Id.Should().Be(category.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            _categoryRepositoryMock.GetById(Arg.Any<int>()).Returns((Category?)null);

            // Act
            var result = await _service.GetById(categoryId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.GetById(category.Id).Returns((Category?)null);

            // Act
            await _service.GetById(category.Id);

            // Assert
            await _categoryRepositoryMock.Received(1).GetById(category.Id);
        }

        [Fact]
        public async Task Add_ShouldAddCategory_WhenCategoryNameDoesNotExist()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _service.Add(category);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OperationResult<Category>>();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Add_ShouldNotAddCategory_WhenCategoryNameAlreadyExists()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _service.Add(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Add_ShouldCallRepositoryOnce_WhenCategoryIsValid()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            await _service.Add(category);

            // Assert
            await _categoryRepositoryMock.Received(1).Add(category, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Update_ShouldUpdateCategory_WhenCategoryNameDoesNotExist()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);
            _categoryRepositoryMock
                .GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Should().BeOfType<OperationResult<Category>>();
        }

        [Fact]
        public async Task Update_ShouldNotUpdateCategory_WhenCategoryNameAlreadyExists()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _categoryRepositoryMock
                .GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Update_ShouldCallRepositoryOnce_WhenCategoryIsValid()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);
            _categoryRepositoryMock
                .GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            // Act
            await _service.Update(category);

            // Assert
            await _categoryRepositoryMock.Received(1).Update(category, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Remove_ShouldRemoveCategory_WhenCategoryHasNoAssociatedBooks()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.GetById(category.Id).Returns(category);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _service.Remove(category.Id);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Remove_ShouldNotRemoveCategory_WhenCategoryHasAssociatedBooks()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.GetById(category.Id).Returns(category);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _service.Remove(category.Id);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("associated books");
        }

        [Fact]
        public async Task Remove_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            _categoryRepositoryMock.GetById(Arg.Any<int>()).Returns((Category?)null);

            // Act
            var result = await _service.Remove(categoryId);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Remove_ShouldCallRepositoryOnce_WhenCategoryCanBeRemoved()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.GetById(category.Id).Returns(category);
            _bookRepositoryMock
                .ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            await _service.Remove(category.Id);

            // Assert
            _categoryRepositoryMock.Received(1).Remove(category);
        }

        [Fact]
        public async Task Search_ShouldReturnListOfCategories_WhenCategoriesWithSearchedNameExist()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var categories = _fixture.Build<Category>()
                .With(c => c.Name, searchTerm)
                .CreateMany(2)
                .ToList();
            _categoryRepositoryMock
                .Search(Arg.Any<Expression<Func<Category, bool>>>())
                .Returns(categories);

            // Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Category>>();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_ShouldReturnNull_WhenCategoriesWithSearchedNameDoNotExist()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            _categoryRepositoryMock
                .Search(Arg.Any<Expression<Func<Category, bool>>>())
                .Returns((IEnumerable<Category>?)null);

            // Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Search_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var searchTerm = _fixture.Create<string>();
            var categories = _fixture.CreateMany<Category>().ToList();
            _categoryRepositoryMock
                .Search(Arg.Any<Expression<Func<Category, bool>>>())
                .Returns(categories);

            // Act
            await _service.Search(searchTerm);

            // Assert
            await _categoryRepositoryMock.Received(1).Search(Arg.Any<Expression<Func<Category, bool>>>());
        }
    }
}
