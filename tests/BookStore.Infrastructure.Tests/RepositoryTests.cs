using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using BookStore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.Infrastructure.Tests
{
    /// <summary>
    /// This is another example of how to create tests for the abstract base class
    /// </summary>
    public class RepositoryTests
    {
        public abstract class RepositoryTestsBase
        {

            protected readonly DbContextOptions<BookStoreDbContext> _options;

            protected RepositoryTestsBase()
            {
                _options = BookStoreHelperTests.BookStoreDbContextOptionsSQLiteInMemory();
                BookStoreHelperTests.CreateDataBaseSQLiteInMemory(_options);
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
                new Category { Id = 1, Name = "Category Test 1" },
                new Category { Id = 2, Name = "Category Test 2" },
                new Category { Id = 3, Name = "Category Test 3" }
            };
            }
        }

        public class GetAll : RepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfCategory_WhenCategoriesExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var categories = await repository.GetAll();

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
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var categories = await repository.GetAll();

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
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var categoryList = await repository.GetAll();

                    // Assert
                    categoryList.Count().Should().Be(3);
                    categoryList.Should().BeEquivalentTo(categoryList);
                }
            }
        }

        public class GetAllWithPagination : RepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnAListOfCategory_WhenCategoriesExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var pagedResponse = await repository.GetAllWithPagination(1, 10);

                    // Assert
                    pagedResponse.Should().NotBeNull();
                    pagedResponse.Should().BeOfType<PagedResponse<Category>>();
                }
            }

            [Fact]
            public async void ShouldReturnAnEmptyList_WhenCategoriesDoNotExist()
            {
                await BookStoreHelperTests.CleanDataBase(_options);

                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var pagedResponse = await repository.GetAllWithPagination(1, 10);

                    // Assert
                    pagedResponse.Should().NotBeNull();
                    pagedResponse.Data.Should().BeEmpty();
                    pagedResponse.Should().BeOfType<PagedResponse<Category>>();
                }
            }

            [Fact]
            public async void ShouldReturnAListOfCategoryWithCorrectValues_WhenCategoriesExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var expectedCategories = CreateCategoryList();
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var pagedResponse = await repository.GetAllWithPagination(1, 10);

                    // Assert
                    pagedResponse.Data.Count().Should().Be(3);
                    pagedResponse.Data.Should().BeEquivalentTo(expectedCategories);
                }
            }
        }


        public class GetById : RepositoryTestsBase
        {
            [Fact]
            public async void ShouldReturnCategoryWithSearchedId_WhenCategoryWithSearchedIdExist()
            {
                await using (var context = new BookStoreDbContext(_options))
                {
                    // Arrange
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var category = await repository.GetById(2);

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
                    var repository = new RepositoryConcreteClass(context);

                    // Act
                    var category = await repository.GetById(1);

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
                    var repository = new RepositoryConcreteClass(context);

                    var expectedCategories = CreateCategoryList();

                    // Act
                    var category = await repository.GetById(2);

                    // Assert
                    category.Should().BeEquivalentTo(expectedCategories[1]);
                }
            }
        }

        public class AddCategory : RepositoryTestsBase
        {
            [Fact]
            public async void ShouldAddCategoryWithCorrectValues_WhenCategoryIsValid()
            {
                // Arrange
                Category categoryToAdd = new Category();

                // Act
                await using (var context = new BookStoreDbContext(_options))
                {
                    var repository = new RepositoryConcreteClass(context);
                    categoryToAdd = CreateCategory();

                    await repository.Add(categoryToAdd);
                }

                // Assert
                await using (var context = new BookStoreDbContext(_options))
                {
                    var categoryResult = await context.Categories.Where(b => b.Id == 4).FirstOrDefaultAsync();

                    categoryResult.Should().NotBeNull();
                    categoryResult.Should().BeOfType<Category>();
                    categoryResult.Should().BeEquivalentTo(categoryToAdd);
                }
            }
        }

        public class UpdateCategory : RepositoryTestsBase
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
                    var repository = new RepositoryConcreteClass(context);
                    await repository.Update(categoryToUpdate);
                }

                // Assert
                await using (var context = new BookStoreDbContext(_options))
                {
                    var updatedCategory = await context.Categories.Where(b => b.Id == 1).FirstOrDefaultAsync();

                    updatedCategory.Should().NotBeNull();
                    updatedCategory.Should().BeOfType<Category>();
                    updatedCategory.Should().BeEquivalentTo(updatedCategory);
                }
            }
        }

        public class Remove : RepositoryTestsBase
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
                    var repository = new RepositoryConcreteClass(context);

                    await repository.Remove(categoryToRemove);
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

    internal class RepositoryConcreteClass : Repository<Category>
    {
        internal RepositoryConcreteClass(BookStoreDbContext bookStoreDbContext) : base(bookStoreDbContext)
        {

        }
    }
}
