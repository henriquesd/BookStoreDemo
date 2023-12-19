using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;

namespace BookStore.API.Configuration
{
    public class DatabaseSeeder
    {
        private readonly BookStoreDbContext _dbContext;

        public DatabaseSeeder(BookStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedData()
        {
            if (!_dbContext.Categories.Any())
            {
                var categories = Enumerable.Range(1, 110).Select(i => new Category { Name = $"Category {i}" });
                _dbContext.Categories.AddRange(categories);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Books.Any() && _dbContext.Categories.Any())
            {
                var books = Enumerable.Range(1, 110).Select(i => new Book {
                    Name = $"Book {i}",
                    CategoryId = _dbContext.Categories.FirstOrDefault()!.Id,
                    Author = $"Author {i}",
                    Description = $"Test {i}" 
                });

                _dbContext.Books.AddRange(books);
                _dbContext.SaveChanges();
            }
        }
    }
}
