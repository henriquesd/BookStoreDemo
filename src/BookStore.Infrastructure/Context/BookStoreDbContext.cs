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

            base.OnModelCreating(modelBuilder);
        }

        private static void SeedEntity<TEntity>(ModelBuilder modelBuilder, int count, Func<int, TEntity> createEntity) 
            where TEntity : class
        {
            var entities = Enumerable.Range(1, count).Select(createEntity).ToList();
            modelBuilder.Entity<TEntity>().HasData(entities);
        }
    }
}