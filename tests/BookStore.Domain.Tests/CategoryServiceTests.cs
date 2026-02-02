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
        private readonly Fixture _fixture;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _fixture = FixtureFactory.Create();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _bookServiceMock = new Mock<IBookService>();
            _service = new CategoryService(_categoryRepositoryMock.Object, _bookServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfCategories_WhenCategoriesExist()
        {
            var categories = _fixture.CreateMany<Category>(3).ToList();
            _categoryRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(categories);

            var result = await _service.GetAll();

            result.Should().NotBeNull();
            result.Should().BeOfType<List<Category>>();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNull_WhenCategoriesDoNotExist()
        {
            _categoryRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync((List<Category>)null);

            var result = await _service.GetAll();

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldCallRepositoryOnce_WhenCalled()
        {
            _categoryRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync((List<Category>)null);

            await _service.GetAll();

            _categoryRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 5)]
        [InlineData(1, 20)]
        public async Task GetAllWithPagination_ShouldReturnPagedResponse_WhenCategoriesExist(int pageNumber, int pageSize)
        {
            var categories = _fixture.CreateMany<Category>(5).ToList();
            var pagedResponse = new PagedResponse<Category>(categories, pageNumber, pageSize, categories.Count);
            _categoryRepositoryMock.Setup(r => r.GetAllWithPagination(pageNumber, pageSize)).ReturnsAsync(pagedResponse);

            var result = await _service.GetAllWithPagination(pageNumber, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<PagedResponse<Category>>();
            result.Data.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldReturnEmptyList_WhenCategoriesDoNotExist()
        {
            var pagedResponse = new PagedResponse<Category>(new List<Category>(), 1, 10, 0);
            _categoryRepositoryMock.Setup(r => r.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

            var result = await _service.GetAllWithPagination(1, 10);

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWithPagination_ShouldCallRepositoryOnce_WhenCalled()
        {
            var pagedResponse = _fixture.Create<PagedResponse<Category>>();
            _categoryRepositoryMock.Setup(r => r.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedResponse);

            await _service.GetAllWithPagination(1, 10);

            _categoryRepositoryMock.Verify(r => r.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetById_ShouldReturnCategory_WhenCategoryExists(int categoryId)
        {
            var category = _fixture.Build<Category>().With(c => c.Id, categoryId).Create();
            _categoryRepositoryMock.Setup(r => r.GetById(categoryId)).ReturnsAsync(category);

            var result = await _service.GetById(categoryId);

            result.Should().NotBeNull();
            result.Should().BeOfType<Category>();
            result.Id.Should().Be(categoryId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            _categoryRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((Category)null);

            var result = await _service.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ShouldCallRepositoryOnce_WhenCalled()
        {
            _categoryRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((Category)null);

            await _service.GetById(1);

            _categoryRepositoryMock.Verify(r => r.GetById(1), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldAddCategory_WhenCategoryNameDoesNotExist()
        {
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepositoryMock.Setup(r => r.Add(category, It.IsAny<CancellationToken>()));

            var result = await _service.Add(category);

            result.Should().NotBeNull();
            result.Should().BeOfType<OperationResult<Category>>();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Add_ShouldNotAddCategory_WhenCategoryNameAlreadyExists()
        {
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.Add(category);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Add_ShouldCallRepositoryOnce_WhenCategoryIsValid()
        {
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepositoryMock.Setup(r => r.Add(category, It.IsAny<CancellationToken>()));

            await _service.Add(category);

            _categoryRepositoryMock.Verify(r => r.Add(category, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldUpdateCategory_WhenCategoryNameDoesNotExist()
        {
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsNoTracking(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _categoryRepositoryMock.Setup(r => r.Update(category, It.IsAny<CancellationToken>()));

            var result = await _service.Update(category);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Payload.Should().NotBeNull();
            result.Should().BeOfType<OperationResult<Category>>();
        }

        [Fact]
        public async Task Update_ShouldNotUpdateCategory_WhenCategoryNameAlreadyExists()
        {
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsNoTracking(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var result = await _service.Update(category);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Update_ShouldCallRepositoryOnce_WhenCategoryIsValid()
        {
            var category = _fixture.Create<Category>();
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsNoTracking(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            await _service.Update(category);

            _categoryRepositoryMock.Verify(r => r.Update(category, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Remove_ShouldRemoveCategory_WhenCategoryHasNoAssociatedBooks()
        {
            var category = _fixture.Create<Category>();

            _categoryRepositoryMock
                .Setup(r => r.GetById(category.Id))
                .ReturnsAsync(category);

            _bookServiceMock
                .Setup(s => s.GetBooksByCategory(category.Id))
                .ReturnsAsync(new List<Book>());

            var result = await _service.Remove(category.Id);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Remove_ShouldNotRemoveCategory_WhenCategoryHasAssociatedBooks()
        {
            var category = _fixture.Create<Category>();

            var books = _fixture
                .Build<Book>()
                .With(b => b.CategoryId, category.Id)
                .CreateMany()
                .ToList();

            _categoryRepositoryMock
                .Setup(r => r.GetById(category.Id))
                .ReturnsAsync(category);

            _bookServiceMock
                .Setup(s => s.GetBooksByCategory(category.Id))
                .ReturnsAsync(books);

            var result = await _service.Remove(category.Id);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("associated books");
        }

        [Fact]
        public async Task Remove_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            _categoryRepositoryMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync((Category)null);

            var result = await _service.Remove(999);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Remove_ShouldCallRepositoryOnce_WhenCategoryCanBeRemoved()
        {
            var category = _fixture.Create<Category>();

            _categoryRepositoryMock
                .Setup(r => r.GetById(category.Id))
                .ReturnsAsync(category);

            _bookServiceMock
                .Setup(s => s.GetBooksByCategory(category.Id))
                .ReturnsAsync(new List<Book>());

            await _service.Remove(category.Id);

            _categoryRepositoryMock.Verify(r => r.Remove(category), Times.Once);
        }

        [Theory]
        [InlineData("Fiction")]
        [InlineData("Science")]
        [InlineData("Biography")]
        public async Task Search_ShouldReturnListOfCategories_WhenCategoriesWithSearchedNameExist(string searchTerm)
        {
            var categories = _fixture
                .Build<Category>()
                .With(c => c.Name, searchTerm)
                .CreateMany(2)
                .ToList();

            _categoryRepositoryMock
                .Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
                .ReturnsAsync(categories);

            var result = await _service.Search(searchTerm);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<Category>>();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_ShouldReturnNull_WhenCategoriesWithSearchedNameDoNotExist()
        {
            _categoryRepositoryMock.Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
                .ReturnsAsync((IEnumerable<Category>)null);

            var result = await _service.Search("NonExistent");

            result.Should().BeNull();
        }

        [Fact]
        public async Task Search_ShouldCallRepositoryOnce_WhenCalled()
        {
            var categories = _fixture.CreateMany<Category>().ToList();
            _categoryRepositoryMock.Setup(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
                .ReturnsAsync(categories);

            await _service.Search("Test");

            _categoryRepositoryMock.Verify(r => r.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()), Times.Once);
        }
    }
}
