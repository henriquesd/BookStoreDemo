using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(BookStoreDbContext context) : base(context) { }

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
                .Where(b => b.Name.Contains(searchedValue) ||
                            b.Author.Contains(searchedValue) ||
                            (b.Description != null && b.Description.Contains(searchedValue)) ||
                            (b.Category != null && b.Category.Name.Contains(searchedValue)))
                .ToListAsync(ct);
        }
    }
}
