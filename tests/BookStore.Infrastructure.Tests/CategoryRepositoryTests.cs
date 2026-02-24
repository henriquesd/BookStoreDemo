using AutoFixture;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using BookStore.Infrastructure.Repositories;
using BookStore.Infrastructure.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.Infrastructure.Tests
{
    public class CategoryRepositoryTests
    {
        public abstract class CategoryRepositoryTestsBase
        {
            protected readonly DbContextOptions<BookStoreDbContext> _options;
            protected readonly Fixture _fixture;

            protected CategoryRepositoryTestsBase()
            {
                _options = BookStoreHelperTests.BookStoreDbContextOptionsSQLiteInMemory();
                BookStoreHelperTests.CreateDataBaseSQLiteInMemory(_options);
                _fixture = FixtureFactory.Create();
            }
        }

        public class GetAll : CategoryRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnAListOfCategory_WhenCategoriesExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var categoryRepository = new CategoryRepository(context);

                // Act
                var categories = await categoryRepository.GetAll();

                // Assert
                categories.Should().NotBeNull();
                categories.Should().BeOfType<List<Category>>();
                categories.Should().HaveCount(3);
            }

            [Fact]
            public async Task ShouldReturnAnEmptyList_WhenCategoriesDoNotExist()
            {
                // Arrange
                await BookStoreHelperTests.CleanDataBase(_options);
                await using var context = new BookStoreDbContext(_options);
                var categoryRepository = new CategoryRepository(context);

                // Act
                var categories = await categoryRepository.GetAll();

                // Assert
                categories.Should().NotBeNull();
                categories.Should().BeEmpty();
                categories.Should().BeOfType<List<Category>>();
            }

            [Fact]
            public async Task ShouldReturnAListOfCategoryWithCorrectValues_WhenCategoriesExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var categoryRepository = new CategoryRepository(context);

                // Act
                var categoryList = await categoryRepository.GetAll();

                // Assert
                categoryList.Should().NotBeNull();
                categoryList.Should().BeOfType<List<Category>>();
                categoryList.Count().Should().Be(3);
                categoryList.Should().AllSatisfy(c => c.Name.Should().StartWith("Category Test"));
            }
        }

        public class GetAllWithPagination : CategoryRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnAListOfPaginatedCategory_WhenCategoriesExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var categoryRepository = new CategoryRepository(context);
                var pageNumber = 1;
                var pageSize = 10;

                // Act
                var pagedResponse = await categoryRepository.GetAllWithPagination(pageNumber, pageSize);

                // Assert
                pagedResponse.Should().NotBeNull();
                pagedResponse.Should().BeOfType<PagedResponse<Category>>();
                pagedResponse.PageNumber.Should().Be(pageNumber);
                pagedResponse.PageSize.Should().Be(pageSize);
            }

            [Fact]
            public async Task ShouldReturnAnEmptyList_WhenCategoriesDoNotExist()
            {
                // Arrange
                await BookStoreHelperTests.CleanDataBase(_options);
                await using var context = new BookStoreDbContext(_options);
                var categoryRepository = new CategoryRepository(context);

                // Act
                var pagedResponse = await categoryRepository.GetAllWithPagination(1, 10);

                // Assert
                pagedResponse.Should().NotBeNull();
                pagedResponse.Data.Should().BeEmpty();
                pagedResponse.Should().BeOfType<PagedResponse<Category>>();
            }

            [Fact]
            public async Task ShouldReturnPagedResponseOfCategoryWithCorrectValues_WhenCategoriesExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var categoryRepository = new CategoryRepository(context);

                // Act
                var pagedResponse = await categoryRepository.GetAllWithPagination(1, 10);

                // Assert
                pagedResponse.Should().NotBeNull();
                pagedResponse.Should().BeOfType<PagedResponse<Category>>();
                pagedResponse.Data.Count().Should().Be(3);
                pagedResponse.Data.Should().AllSatisfy(c => c.Name.Should().StartWith("Category Test"));
            }
        }

        public class GetById : CategoryRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldReturnCategoryWithSearchedId_WhenCategoryWithSearchedIdExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var categoryRepository = new CategoryRepository(context);
                var existingCategoryId = 2;

                // Act
                var category = await categoryRepository.GetById(existingCategoryId);

                // Assert
                category.Should().NotBeNull();
                category.Should().BeOfType<Category>();
                category!.Id.Should().Be(existingCategoryId);
            }

            [Fact]
            public async Task ShouldReturnNull_WhenCategoryWithSearchedIdDoesNotExist()
            {
                // Arrange
                await BookStoreHelperTests.CleanDataBase(_options);
                await using var context = new BookStoreDbContext(_options);
                var categoryRepository = new CategoryRepository(context);
                var nonExistentCategoryId = _fixture.Create<int>();

                // Act
                var category = await categoryRepository.GetById(nonExistentCategoryId);

                // Assert
                category.Should().BeNull();
            }

            [Fact]
            public async Task ShouldReturnCategoryWithCorrectValues_WhenCategoryExist()
            {
                await using var context = new BookStoreDbContext(_options);

                // Arrange
                var categoryRepository = new CategoryRepository(context);
                var existingCategoryId = 2;

                // Act
                var category = await categoryRepository.GetById(existingCategoryId);

                // Assert
                category.Should().NotBeNull();
                category!.Id.Should().Be(existingCategoryId);
                category.Name.Should().Be("Category Test 2");
            }
        }

        public class AddCategory : CategoryRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldAddCategoryWithCorrectValues_WhenCategoryIsValid()
            {
                // Arrange
                var categoryToAdd = _fixture
                    .Build<Category>()
                    .With(c => c.Id, 100)
                    .Create();

                // Act
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryRepository = new CategoryRepository(context);
                    await categoryRepository.Add(categoryToAdd);
                }

                // Assert
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryResult = await context.Categories.Where(b => b.Id == categoryToAdd.Id).FirstOrDefaultAsync();

                    categoryResult.Should().NotBeNull();
                    categoryResult!.Name.Should().Be(categoryToAdd.Name);
                    categoryResult.Should().BeOfType<Category>();
                }
            }
        }

        public class UpdateCategory : CategoryRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldUpdateCategoryWithCorrectValues_WhenCategoryIsValid()
            {
                // Arrange
                var updatedName = _fixture.Create<string>();
                Category categoryToUpdate;
                await using (var context = new BookStoreDbContext(_options))
                {
                    categoryToUpdate = (await context.Categories.Where(b => b.Id == 1).FirstOrDefaultAsync())!;
                    categoryToUpdate.Name = updatedName;
                }

                // Act
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryRepository = new CategoryRepository(context);
                    await categoryRepository.Update(categoryToUpdate);
                }

                // Assert
                await using (var context = new BookStoreDbContext(_options))
                {
                    var updatedCategory = await context.Categories.Where(b => b.Id == 1).FirstOrDefaultAsync();

                    updatedCategory.Should().NotBeNull();
                    updatedCategory.Should().BeOfType<Category>();
                    updatedCategory!.Name.Should().Be(updatedName);
                }
            }
        }

        public class Remove : CategoryRepositoryTestsBase
        {
            [Fact]
            public async Task ShouldRemoveCategory_WhenCategoryIsValid()
            {
                // Arrange
                var categoryIdToRemove = 2;
                Category categoryToRemove;
                await using (var context = new BookStoreDbContext(_options))
                {
                    categoryToRemove = (await context.Categories.Where(c => c.Id == categoryIdToRemove).FirstOrDefaultAsync())!;
                }

                // Act
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryRepository = new CategoryRepository(context);
                    await categoryRepository.Remove(categoryToRemove);
                }

                // Assert
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryRemoved = await context.Categories.Where(c => c.Id == categoryIdToRemove).FirstOrDefaultAsync();
                    categoryRemoved.Should().BeNull();
                }
            }
        }
    }
}
