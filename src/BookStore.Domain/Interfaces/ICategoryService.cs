using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
        Task<PagedResponse<Category>> GetAllWithPagination(int pageNumber, int pageSize);
        Task<Category> GetById(int id);
        Task<IOperationResult<Category>> Add(Category category);
        Task<IOperationResult<Category>> Update(Category category);
        Task<IOperationResult<bool>> Remove(int id);
        Task<IEnumerable<Category>> Search(string categoryName);
    }
}