using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        new Task<IEnumerable<Book>> GetAll(CancellationToken ct = default);
        new Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default);
        new Task<Book?> GetById(int id, CancellationToken ct = default);
        Task<IEnumerable<Book>> GetBooksByCategory(int categoryId, CancellationToken ct = default);
        Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default);
    }
}