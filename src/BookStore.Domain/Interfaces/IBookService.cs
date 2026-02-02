using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAll(CancellationToken ct = default);
        Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<Book?> GetById(int id, CancellationToken ct = default);
        Task<IOperationResult<Book>> Add(Book book, CancellationToken ct = default);
        Task<IOperationResult<Book>> Update(Book book, CancellationToken ct = default);
        Task<IOperationResult<bool>> Remove(int id, CancellationToken ct = default);
        Task<IEnumerable<Book>> GetBooksByCategory(int categoryId, CancellationToken ct = default);
        Task<IEnumerable<Book>> Search(string bookName, CancellationToken ct = default);
        Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default);
    }
}