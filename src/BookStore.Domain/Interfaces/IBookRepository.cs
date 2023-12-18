using BookStore.Domain.Models;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.Domain.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        new Task<List<Book>> GetAll();
        Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize);
        new Task<Book> GetById(int id);
        Task<IEnumerable<Book>> GetBooksByCategory(int categoryId);
        Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue);
    }
}