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

            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            return new PagedResponse<Category>
            {
                Data = categories,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalItems
            };
        }
    }
}