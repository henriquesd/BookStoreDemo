using BookStore.Domain.Models;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.Domain.Interfaces
{
    public interface ICategoryService : IDisposable
    {
        Task<IEnumerable<Category>> GetAll();
        Task<PagedResponse<Category>> GetAllWithPagination(int pageNumber, int pageSize);
        Task<Category> GetById(int id);
        Task<IOperationResult<Category>> Add(Category category);
        Task<IOperationResult<Category>> Update(Category category);
        Task<bool> Remove(Category category);
        Task<IEnumerable<Category>> Search(string categoryName);
    }
}