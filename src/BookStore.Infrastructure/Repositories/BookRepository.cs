using System.Linq.Expressions;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public override async Task<IEnumerable<Book>> GetAll(CancellationToken ct = default)
        {
            return await Db.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .OrderBy(b => b.Name)
                .ToListAsync(ct);
        }

        public override async Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var totalRecords = await Db.Books.AsNoTracking().CountAsync(ct);

            var books = await Db.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var pagedResponse = new PagedResponse<Book>(books, pageNumber, pageSize, totalRecords);

            return pagedResponse;
        }

        public override async Task<Book?> GetById(int id, CancellationToken ct = default)
        {
            return await Db.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync(ct);
        }

        public override async Task<PagedResponse<Book>> SearchWithPagination(Expression<Func<Book, bool>> predicate, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = Db.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(predicate);

            var totalRecords = await query.CountAsync(ct);

            var books = await query
                .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResponse<Book>(books, pageNumber, pageSize, totalRecords);
        }

        public async Task<IEnumerable<Book>> GetBooksByCategory(int categoryId, CancellationToken ct = default)
        {
            return await Db.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default)
        {
            return await Db.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(CreateSearchPredicate(searchedValue))
                .ToListAsync(ct);
        }

        public async Task<PagedResponse<Book>> SearchBookWithCategoryPagination(string searchedValue, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = Db.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(CreateSearchPredicate(searchedValue));

            var totalRecords = await query.CountAsync(ct);

            var books = await query
                .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResponse<Book>(books, pageNumber, pageSize, totalRecords);
        }

        public BookRepository(BookStoreDbContext context) : base(context) { }

        /// <summary>
        /// Creates a search predicate that searches across multiple book fields including category name.
        /// </summary>
        /// <param name="searchValue">The value to search for</param>
        /// <returns>An expression that can be used in LINQ queries</returns>
        private static Expression<Func<Book, bool>> CreateSearchPredicate(string searchValue)
        {
            return b => b.Name.Contains(searchValue) ||
                        b.Author.Contains(searchValue) ||
                        (b.Description != null && b.Description.Contains(searchValue)) ||
                        (b.Category != null && b.Category.Name.Contains(searchValue));
        }

    }
}
