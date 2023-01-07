using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using BookStore.Infrastructure.Repositories;
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

            /// <summary>
            /// The CategoryRepository class only use the methods from the Repository base class
            /// </summary>
            protected CategoryRepositoryTestsBase()
            {
                // Use this when using a SQLite InMemory database
                _options = BookStoreHelperTests.BookStoreDbContextOptionsSQLiteInMemory();
                BookStoreHelperTests.CreateDataBaseSQLiteInMemory(_options);

                // Use this when using a EF Core InMemory database
                //_options = BookStoreHelperTests.BookStoreDbContextOptionsEfCoreInMemory();
                //BookStoreHelperTests.CreateDataBaseEfCoreInMemory(_options);
            }

            protected Category CreateCategory()
            {
                return new Category()
                {
                    Id = 4,
                    Name = "Category Test 4",
                };
            }

            protected List<Category> CreateCategoryList()
            {
                return new List<Category>()
            {
                new Category {Id = 1, Name = "Category Test 1"},
                new Category {Id = 2, Name = "Category Test 2"},
                new Category {Id = 3, Name = "Category Test 3"}
            };
            }
        }

        public class GetAll : CategoryRepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfCategory_WhenCategoriesExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var categoryRepository = new CategoryRepository(context);

                    // Act
                    var categories = await categoryRepository.GetAll();

                    // Assert
                    categories.Should().NotBeNull();
                    categories.Should().BeOfType<List<Category>>();
                }
            }

            [Fact]
            public async void ShouldReturnAnEmptyList_WhenCategoriesDoNotExist()
            {
                await BookStoreHelperTests.CleanDataBase(_options);

                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var categoryRepository = new CategoryRepository(context);

                    // Act
                    var categories = await categoryRepository.GetAll();

                    // Assert
                    categories.Should().NotBeNull();
                    categories.Should().BeEmpty();
                    categories.Should().BeOfType<List<Category>>();
                }
            }

            [Fact]
            public async void ShouldReturnAListOfCategoryWithCorrectValues_WhenCategoriesExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var expectedCategories = CreateCategoryList();
                    var categoryRepository = new CategoryRepository(context);

                    // Act
                    var categoryList = await categoryRepository.GetAll();

                    // Assert
                    categoryList.Should().NotBeNull();
                    categoryList.Should().BeOfType<List<Category>>();
                    categoryList.Count().Should().Be(3);
                    categoryList.Should().BeEquivalentTo(expectedCategories);
                }
            }
        }

        public class GetById : CategoryRepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnCategoryWithSearchedId_WhenCategoryWithSearchedIdExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var categoryRepository = new CategoryRepository(context);

                    // Act
                    var category = await categoryRepository.GetById(2);

                    // Assert
                    category.Should().NotBeNull();
                    category.Should().BeOfType<Category>();
                }
            }

            [Fact]
            public async void ShouldReturnNull_WhenCategoryWithSearchedIdDoesNotExist()
            {
                await BookStoreHelperTests.CleanDataBase(_options);

                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var categoryRepository = new CategoryRepository(context);

                    // Act
                    var category = await categoryRepository.GetById(1);

                    // Assert
                    category.Should().BeNull();
                }
            }

            [Fact]
            public async void ShouldReturnCategoryWithCorrectValues_WhenCategoryExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var categoryRepository = new CategoryRepository(context);

                    // Act
                    var expectedCategories = CreateCategoryList();
                    var category = await categoryRepository.GetById(2);

                    // Assert
                    category.Should().BeEquivalentTo(expectedCategories[1]);
                }
            }
        }

        public class AddCategory : CategoryRepositoryTestsBase
        {
            [Fact]
            public async void ShouldAddCategoryWithCorrectValues_WhenCategoryIsValid()
            {
                // Arrange
                Category categoryToAdd = new Category();

                // Act
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryRepository = new CategoryRepository(context);
                    categoryToAdd = CreateCategory();

                    await categoryRepository.Add(categoryToAdd);
                }

                // Assert
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryResult = await context.Categories.Where(b => b.Id == 4).FirstOrDefaultAsync();

                    categoryResult.Should().NotBeNull();
                    categoryResult.Should().BeEquivalentTo(categoryToAdd);
                    categoryResult.Should().NotBeNull();
                    categoryResult.Should().BeOfType<Category>();
                }
            }
        }

        public class UpdateCategory : CategoryRepositoryTestsBase
        {
            [Fact]
            public async void ShouldUpdateCategoryWithCorrectValues_WhenCategoryIsValid()
            {
                // Arrange
                Category categoryToUpdate = new Category();
                await using (var context = new BookStoreDbContext(_options))
                {
                    categoryToUpdate = await context.Categories.Where(b => b.Id == 1).FirstOrDefaultAsync();
                    categoryToUpdate.Name = "Updated Name";
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
                    updatedCategory.Should().BeEquivalentTo(categoryToUpdate);
                }
            }
        }

        public class Remove : CategoryRepositoryTestsBase
        {
            [Fact]
            public async void ShouldRemoveCategory_WhenCategoryIsValid()
            {
                // Arrange
                Category categoryToRemove = new Category();

                await using (var context = new BookStoreDbContext(_options))
                {
                    categoryToRemove = await context.Categories.Where(c => c.Id == 2).FirstOrDefaultAsync();
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
                    var categoryRemoved = await context.Categories.Where(c => c.Id == 2).FirstOrDefaultAsync();

                    categoryRemoved.Should().BeNull();
                }
            }
        }
    }
}