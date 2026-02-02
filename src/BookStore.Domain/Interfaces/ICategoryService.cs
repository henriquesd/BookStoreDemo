using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll(CancellationToken ct = default);
        Task<PagedResponse<Category>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<Category?> GetById(int id, CancellationToken ct = default);
        Task<IOperationResult<Category>> Add(Category category, CancellationToken ct = default);
        Task<IOperationResult<Category>> Update(Category category, CancellationToken ct = default);
        Task<IOperationResult<bool>> Remove(int id, CancellationToken ct = default);
        Task<IEnumerable<Category>> Search(string categoryName, CancellationToken ct = default);
    }
}