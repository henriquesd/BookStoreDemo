using System.Linq.Expressions;
using AutoFixture;
using BookStore.Domain.Constants;
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
        public void Constructor_Should_ThrowArgumentNullException_When_CategoryRepositoryIsNull()
        {
            // Arrange & Act
            Action act = () => new CategoryService(null!, _bookRepositoryMock);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("categoryRepository");
        }

        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_BookRepositoryIsNull()
        {
            // Arrange & Act
            Action act = () => new CategoryService(_categoryRepositoryMock, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("bookRepository");
        }

        [Fact]
        public async Task GetAll_Should_ReturnListOfCategories_When_CategoriesExist()
        {
            // Arrange
            var categories = _fixture.CreateMany<Category>(3).ToList();
            _categoryRepositoryMock.GetAll(Arg.Any<CancellationToken>()).Returns(categories);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_Should_CallRepositoryOnce_When_Called()
        {
            // Arrange
            _categoryRepositoryMock.GetAll(Arg.Any<CancellationToken>()).Returns(new List<Category>());

            // Act
            await _service.GetAll();

            // Assert
            await _categoryRepositoryMock.Received(1).GetAll(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllWithPagination_Should_ReturnPagedResponse_When_CategoriesExist()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var categories = _fixture.CreateMany<Category>(5).ToList();
            var pagedResponse = new PagedResponse<Category>(categories, pageNumber, pageSize, categories.Count);
            _categoryRepositoryMock.GetAllWithPagination(pageNumber, pageSize, Arg.Any<CancellationToken>()).Returns(pagedResponse);

            // Act
            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Data.Should().HaveCount(5);
            result.Payload.PageNumber.Should().Be(pageNumber);
            result.Payload.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task GetAllWithPagination_Should_CallRepositoryOnce_When_Called()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var pagedResponse = _fixture.Create<PagedResponse<Category>>();
            _categoryRepositoryMock.GetAllWithPagination(pageNumber, pageSize, Arg.Any<CancellationToken>()).Returns(pagedResponse);

            // Act
            await _service.GetAllWithPagination(pageNumber, pageSize);

            // Assert
            await _categoryRepositoryMock.Received(1).GetAllWithPagination(pageNumber, pageSize, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetById_Should_ReturnCategory_When_CategoryExists()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.GetById(category.Id, Arg.Any<CancellationToken>()).Returns(category);

            // Act
            var result = await _service.GetById(category.Id);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload!.Id.Should().Be(category.Id);
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound_When_CategoryDoesNotExist()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            _categoryRepositoryMock.GetById(categoryId, Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _service.GetById(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        }

        [Fact]
        public async Task GetById_Should_CallRepositoryOnce_When_Called()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.GetById(categoryId, Arg.Any<CancellationToken>()).Returns(category);

            // Act
            await _service.GetById(categoryId);

            // Assert
            await _categoryRepositoryMock.Received(1).GetById(categoryId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Add_Should_AddCategory_When_CategoryIsValid()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Name, "Valid Category Name")
                .Create();
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.Add(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().Be(category);
            result.ErrorCode.Should().Be(OperationErrorCode.None);
        }

        [Fact]
        public async Task Add_Should_ReturnValidationError_When_CategoryIsNull()
        {
            // Arrange & Act
            var result = await _service.Add(null!);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.EntityCannotBeNull, "Category"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Add_Should_ReturnValidationError_When_CategoryNameIsInvalid(string? invalidName)
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Name, invalidName!)
                .Create();

            // Act
            var result = await _service.Add(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.FieldRequired, "Category name"));
        }

        [Fact]
        public async Task Add_Should_ReturnDuplicateError_When_CategoryNameAlreadyExists()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Name, "Existing Category")
                .Create();
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _service.Add(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.Duplicate);
            result.Message.Should().Be(ErrorMessages.CategoryDuplicate);
        }

        [Fact]
        public async Task Add_Should_CallRepositoryAdd_When_CategoryIsValid()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Name, "Valid Category")
                .Create();
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            await _service.Add(category);

            // Assert
            await _categoryRepositoryMock.Received(1).Add(category, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Add_Should_ReturnError_When_RepositoryThrowsException()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Name, "Valid Category")
                .Create();
            var exceptionMessage = "Database error";
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);
            _categoryRepositoryMock.Add(category, Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception(exceptionMessage)));

            // Act
            var result = await _service.Add(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.UnexpectedError);
            result.Message.Should().Be(string.Format(ErrorMessages.CategoryAddError, exceptionMessage));
        }

        [Fact]
        public async Task Update_Should_UpdateCategory_When_CategoryIsValid()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Id, 1)
                .With(c => c.Name, "Updated Category")
                .Create();
            _categoryRepositoryMock.GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().Be(category);
            result.ErrorCode.Should().Be(OperationErrorCode.None);
        }

        [Fact]
        public async Task Update_Should_ReturnValidationError_When_CategoryIsNull()
        {
            // Arrange & Act
            var result = await _service.Update(null!);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.EntityCannotBeNull, "Category"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Update_Should_ReturnValidationError_When_CategoryNameIsInvalid(string? invalidName)
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Id, 1)
                .With(c => c.Name, invalidName!)
                .Create();

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.FieldRequired, "Category name"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task Update_Should_ReturnValidationError_When_CategoryIdIsInvalid(int invalidId)
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Id, invalidId)
                .With(c => c.Name, "Valid Name")
                .Create();

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.InvalidId, "category"));
        }

        [Fact]
        public async Task Update_Should_ReturnNotFound_When_CategoryDoesNotExist()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Id, 1)
                .With(c => c.Name, "Valid Category")
                .Create();
            _categoryRepositoryMock.GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Message.Should().Be(string.Format(ErrorMessages.CategoryNotFound, category.Id));
        }

        [Fact]
        public async Task Update_Should_ReturnDuplicateError_When_CategoryNameExistsForDifferentCategory()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Id, 1)
                .With(c => c.Name, "Duplicate Name")
                .Create();
            _categoryRepositoryMock.GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.Duplicate);
            result.Message.Should().Be(ErrorMessages.CategoryDuplicate);
        }

        [Fact]
        public async Task Update_Should_CallRepositoryUpdate_When_CategoryIsValid()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Id, 1)
                .With(c => c.Name, "Valid Category")
                .Create();
            _categoryRepositoryMock.GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            await _service.Update(category);

            // Assert
            await _categoryRepositoryMock.Received(1).Update(category, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Update_Should_ReturnError_When_RepositoryThrowsException()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .With(c => c.Id, 1)
                .With(c => c.Name, "Valid Category")
                .Create();
            var exceptionMessage = "Database error";
            _categoryRepositoryMock.GetByIdAsNoTracking(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _categoryRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);
            _categoryRepositoryMock.Update(category, Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception(exceptionMessage)));

            // Act
            var result = await _service.Update(category);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.UnexpectedError);
            result.Message.Should().Be(string.Format(ErrorMessages.CategoryUpdateError, exceptionMessage));
        }

        [Fact]
        public async Task Remove_Should_RemoveCategory_When_CategoryExistsAndHasNoDependencies()
        {
            // Arrange
            var categoryId = 1;
            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();
            _categoryRepositoryMock.GetById(categoryId, Arg.Any<CancellationToken>()).Returns(category);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.Remove(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().BeTrue();
            result.ErrorCode.Should().Be(OperationErrorCode.None);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task Remove_Should_ReturnValidationError_When_CategoryIdIsInvalid(int invalidId)
        {
            // Arrange & Act
            var result = await _service.Remove(invalidId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            result.Message.Should().Be(string.Format(ErrorMessages.InvalidId, "category"));
        }

        [Fact]
        public async Task Remove_Should_ReturnNotFound_When_CategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 1;
            _categoryRepositoryMock.GetById(categoryId, Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _service.Remove(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Message.Should().Be(string.Format(ErrorMessages.CategoryNotFound, categoryId));
        }

        [Fact]
        public async Task Remove_Should_ReturnHasDependenciesError_When_CategoryHasAssociatedBooks()
        {
            // Arrange
            var categoryId = 1;
            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();
            _categoryRepositoryMock.GetById(categoryId, Arg.Any<CancellationToken>()).Returns(category);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _service.Remove(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.HasDependencies);
            result.Message.Should().Be(ErrorMessages.CategoryHasDependencies);
        }

        [Fact]
        public async Task Remove_Should_CallRepositoryRemove_When_CategoryCanBeRemoved()
        {
            // Arrange
            var categoryId = 1;
            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();
            _categoryRepositoryMock.GetById(categoryId, Arg.Any<CancellationToken>()).Returns(category);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            await _service.Remove(categoryId);

            // Assert
            await _categoryRepositoryMock.Received(1).Remove(category, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Remove_Should_ReturnError_When_RepositoryThrowsException()
        {
            // Arrange
            var categoryId = 1;
            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();
            var exceptionMessage = "Database error";
            _categoryRepositoryMock.GetById(categoryId, Arg.Any<CancellationToken>()).Returns(category);
            _bookRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Book, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);
            _categoryRepositoryMock.Remove(category, Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception(exceptionMessage)));

            // Act
            var result = await _service.Remove(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.UnexpectedError);
            result.Message.Should().Be(string.Format(ErrorMessages.CategoryRemoveError, exceptionMessage));
        }

        [Fact]
        public async Task Search_Should_ReturnCategories_When_MatchingCategoriesExist()
        {
            // Arrange
            var searchTerm = "Fiction";
            var categories = _fixture.Build<Category>()
                .With(c => c.Name, $"{searchTerm} Category")
                .CreateMany(2)
                .ToList();
            _categoryRepositoryMock.Search(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(categories);

            // Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Payload.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_Should_ReturnValidationError_When_SearchTermIsNull()
        {
            // Arrange & Act
            var result = await _service.Search(null!);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Search_Should_ReturnValidationError_When_SearchTermIsWhitespace(string searchTerm)
        {
            // Arrange & Act
            var result = await _service.Search(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
        }

        [Fact]
        public async Task Search_Should_CallRepositorySearch_When_SearchTermIsValid()
        {
            // Arrange
            var searchTerm = "Fiction";
            var categories = _fixture.CreateMany<Category>(2).ToList();
            _categoryRepositoryMock.Search(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>()).Returns(categories);

            // Act
            await _service.Search(searchTerm);

            // Assert
            await _categoryRepositoryMock.Received(1).Search(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Search_Should_NotCallRepository_When_SearchTermIsInvalid()
        {
            // Arrange & Act
            await _service.Search("   ");

            // Assert
            await _categoryRepositoryMock.DidNotReceive().Search(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>());
        }
    }
}
