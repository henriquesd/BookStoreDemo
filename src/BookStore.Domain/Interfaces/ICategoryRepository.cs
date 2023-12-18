using BookStore.Domain.Models;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PagedResponse<Category>> GetAllWithPagination(int pageNumber, int pageSize);
    }
}