using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface ICategoryService
    {
        Task<IOperationResult<IEnumerable<Category>>> GetAll(CancellationToken ct = default);
        Task<IOperationResult<PagedResponse<Category>>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<IOperationResult<Category>> GetById(int id, CancellationToken ct = default);
        Task<IOperationResult<Category>> Add(Category category, CancellationToken ct = default);
        Task<IOperationResult<Category>> Update(Category category, CancellationToken ct = default);
        Task<IOperationResult<bool>> Remove(int id, CancellationToken ct = default);
        Task<IOperationResult<IEnumerable<Category>>> Search(string categoryName, CancellationToken ct = default);
        Task<IOperationResult<PagedResponse<Category>>> SearchWithPagination(string categoryName, int pageNumber, int pageSize, CancellationToken ct = default);
    }
}