using System.Linq;
using BookStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Context
{
    public class BookStoreDbContext : DbContext
    {
        public BookStoreDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                    .Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(150)");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookStoreDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            var categories = new List<Category>();
            for (int i = 1; i <= 110; i++)
            {
                categories.Add(new Category { Id = i, Name = $"Category {i}" });
            }

            var books = new List<Book>();
            for (int i = 1; i <= 110; i++)
            {
                books.Add(new Book { Id = i, Name = $"Book {i}", CategoryId = 1, Author = $"Author {i}", Description = $"Test {i}" });
            }

            modelBuilder.Entity<Category>().HasData(categories);
            modelBuilder.Entity<Book>().HasData(books);

            base.OnModelCreating(modelBuilder);
        }
    }
}