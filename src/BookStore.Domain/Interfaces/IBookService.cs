using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAll();
        Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize);
        Task<Book> GetById(int id);
        Task<IOperationResult<Book>> Add(Book book);
        Task<IOperationResult<Book>> Update(Book book);
        Task<IOperationResult<bool>> Remove(int id);
        Task<IEnumerable<Book>> GetBooksByCategory(int categoryId);
        Task<IEnumerable<Book>> Search(string bookName);
        Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue);
    }
}