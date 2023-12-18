using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.Infrastructure.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(BookStoreDbContext context) : base(context) { }

        public override async Task<List<Book>> GetAll()
        {
            return await Db.Books.AsNoTracking().Include(b => b.Category)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize)
        {
            var totalItems = await Db.Books.AsNoTracking().CountAsync();

            var books = await Db.Books.AsNoTracking()
                .Include(b => b.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<Book>(books, pageNumber, totalItems, pageSize);

            return pagedResponse;
        }

        public override async Task<Book> GetById(int id)
        {
            return await Db.Books.AsNoTracking().Include(b => b.Category)
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategory(int categoryId)
        {
            return await Search(b => b.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue)
        {
            return await Db.Books.AsNoTracking()
                .Include(b => b.Category)
                .Where(b => b.Name.Contains(searchedValue) || 
                            b.Author.Contains(searchedValue) ||
                            b.Description.Contains(searchedValue) ||
                            b.Category.Name.Contains(searchedValue))
                .ToListAsync();
        }
    }
}