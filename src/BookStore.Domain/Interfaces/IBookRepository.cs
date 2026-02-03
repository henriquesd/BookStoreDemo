using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByCategory(int categoryId, CancellationToken ct = default);
        Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default);
    }
}