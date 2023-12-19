using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IBookService : IDisposable
    {
        Task<IEnumerable<Book>> GetAll();
        Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize);
        Task<Book> GetById(int id);
        Task<Book> Add(Book book);
        Task<Book> Update(Book book);
        Task<bool> Remove(Book book);
        Task<IEnumerable<Book>> GetBooksByCategory(int categoryId);
        Task<IEnumerable<Book>> Search(string bookName);
        Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue);
    }
}