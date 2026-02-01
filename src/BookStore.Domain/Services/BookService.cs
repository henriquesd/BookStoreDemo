using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.Domain.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            return await _bookRepository.GetAll();
        }

        public async Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize)
        {
            return await _bookRepository.GetAllWithPagination(pageNumber, pageSize);
        }

        public async Task<Book> GetById(int id)
        {
            return await _bookRepository.GetById(id);
        }

        public async Task<IOperationResult<Book>> Add(Book book)
        {
            try
            {
                var validation = ValidateBook(book);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingBooks = await _bookRepository.Search(b => b.Name == book.Name);
                if (existingBooks.Any())
                {
                    return new OperationResult<Book>(false, "A book with this name already exists");
                }

                await _bookRepository.Add(book);
                return new OperationResult<Book>(book);
            }
            catch (Exception ex)
            {
                return new OperationResult<Book>(false, $"An error occurred while adding the book: {ex.Message}");
            }
        }

        public async Task<IOperationResult<Book>> Update(Book book)
        {
            try
            {
                var validation = ValidateBookForUpdate(book);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingBook = await _bookRepository.GetByIdAsNoTracking(book.Id);
                if (existingBook == null)
                {
                    return new OperationResult<Book>(false, $"Book with ID {book.Id} not found");
                }

                var duplicateBooks = await _bookRepository.Search(b => b.Name == book.Name && b.Id != book.Id);
                if (duplicateBooks.Any())
                {
                    return new OperationResult<Book>(false, "Another book with this name already exists");
                }

                await _bookRepository.Update(book);
                return new OperationResult<Book>(book);
            }
            catch (Exception ex)
            {
                return new OperationResult<Book>(false, $"An error occurred while updating the book: {ex.Message}");
            }
        }

        public async Task<IOperationResult<bool>> Remove(int id)
        {
            try
            {
                var validation = ValidateId(id);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingBook = await _bookRepository.GetById(id);
                if (existingBook == null)
                {
                    return new OperationResult<bool>(false, $"Book with ID {id} not found");
                }

                await _bookRepository.Remove(existingBook);
                return new OperationResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new OperationResult<bool>(false, $"An error occurred while removing the book: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByCategory(int categoryId)
        {
            return await _bookRepository.GetBooksByCategory(categoryId);
        }

        public async Task<IEnumerable<Book>> Search(string bookName)
        {
            return await _bookRepository.Search(c => c.Name.Contains(bookName));
        }

        public async Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue)
        {
            return await _bookRepository.SearchBookWithCategory(searchedValue);
        }

        #region Private Validation Methods

        private OperationResult<Book> ValidateBook(Book book)
        {
            if (book == null)
            {
                return new OperationResult<Book>(false, "Book cannot be null");
            }

            if (string.IsNullOrWhiteSpace(book.Name))
            {
                return new OperationResult<Book>(false, "Book name is required");
            }

            return new OperationResult<Book>(true, null);
        }

        private OperationResult<Book> ValidateBookForUpdate(Book book)
        {
            var basicValidation = ValidateBook(book);
            if (!basicValidation.Success)
            {
                return basicValidation;
            }

            if (book.Id <= 0)
            {
                return new OperationResult<Book>(false, "Invalid book ID");
            }

            return new OperationResult<Book>(true, null);
        }

        private OperationResult<bool> ValidateId(int id)
        {
            if (id <= 0)
            {
                return new OperationResult<bool>(false, "Invalid book ID");
            }

            return new OperationResult<bool>(true, null);
        }

        #endregion
    }
}