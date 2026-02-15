using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IBookService
    {
        Task<IOperationResult<IEnumerable<Book>>> GetAll(CancellationToken ct = default);
        Task<IOperationResult<PagedResponse<Book>>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<IOperationResult<Book>> GetById(int id, CancellationToken ct = default);
        Task<IOperationResult<Book>> Add(Book book, CancellationToken ct = default);
        Task<IOperationResult<Book>> Update(Book book, CancellationToken ct = default);
        Task<IOperationResult<bool>> Remove(int id, CancellationToken ct = default);
        Task<IOperationResult<IEnumerable<Book>>> GetBooksByCategory(int categoryId, CancellationToken ct = default);
        Task<IOperationResult<IEnumerable<Book>>> Search(string bookName, CancellationToken ct = default);
        Task<IOperationResult<PagedResponse<Book>>> SearchWithPagination(string bookName, int pageNumber, int pageSize, CancellationToken ct = default);
        Task<IOperationResult<IEnumerable<Book>>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default);
        Task<IOperationResult<PagedResponse<Book>>> SearchBookWithCategoryPagination(string searchedValue, int pageNumber, int pageSize, CancellationToken ct = default);
    }
}