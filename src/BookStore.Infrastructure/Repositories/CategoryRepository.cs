using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(BookStoreDbContext context) : base(context) { }

        public async Task<PagedResponse<Category>> GetAllWithPagination(int pageNumber, int pageSize)
        {
            var totalItems = await Db.Categories.AsNoTracking().CountAsync();

            var categories = await Db.Categories.AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<Category>(categories, pageNumber, totalItems, pageSize);

            return pagedResponse;
        }
    }
}