using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Configuration
{
    public class DatabaseSeeder
    {
        private readonly BookStoreDbContext _dbContext;

        public DatabaseSeeder(BookStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            if (!await _dbContext.Categories.AnyAsync(ct))
            {
                var categories = Enumerable.Range(1, 110).Select(i => new Category { Name = $"Category {i}" });
                _dbContext.Categories.AddRange(categories);
                await _dbContext.SaveChangesAsync(ct);
            }

            if (!await _dbContext.Books.AnyAsync(ct) && await _dbContext.Categories.AnyAsync(ct))
            {
                var firstCategory = await _dbContext.Categories.FirstAsync(ct);
                var books = Enumerable.Range(1, 110).Select(i => new Book
                {
                    Name = $"Book {i}",
                    CategoryId = firstCategory.Id,
                    Author = $"Author {i}",
                    Description = $"Test {i}",
                    Value = 9.99m + i,
                    PublishDate = DateTime.UtcNow.AddDays(-i)
                });

                _dbContext.Books.AddRange(books);
                await _dbContext.SaveChangesAsync(ct);
            }
        }
    }
}
