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

        public async Task<IEnumerable<Book>> GetAll(CancellationToken ct = default)
        {
            return await _bookRepository.GetAll(ct);
        }

        public async Task<PagedResponse<Book>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            return await _bookRepository.GetAllWithPagination(pageNumber, pageSize, ct);
        }

        public async Task<Book?> GetById(int id, CancellationToken ct = default)
        {
            return await _bookRepository.GetById(id, ct);
        }

        public async Task<IOperationResult<Book>> Add(Book book, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidateBook(book);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingBooks = await _bookRepository.Search(b => b.Name == book.Name, ct);
                if (existingBooks.Any())
                {
                    return OperationResult<Book>.Duplicate("A book with this name already exists");
                }

                await _bookRepository.Add(book, ct);
                return OperationResult<Book>.Ok(book);
            }
            catch (Exception ex)
            {
                return OperationResult<Book>.Error($"An error occurred while adding the book: {ex.Message}");
            }
        }

        public async Task<IOperationResult<Book>> Update(Book book, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidateBookForUpdate(book);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingBook = await _bookRepository.GetByIdAsNoTracking(book.Id, ct);
                if (existingBook == null)
                {
                    return OperationResult<Book>.NotFound($"Book with ID {book.Id} not found");
                }

                var duplicateBooks = await _bookRepository.Search(b => b.Name == book.Name && b.Id != book.Id, ct);
                if (duplicateBooks.Any())
                {
                    return OperationResult<Book>.Duplicate("Another book with this name already exists");
                }

                await _bookRepository.Update(book, ct);
                return OperationResult<Book>.Ok(book);
            }
            catch (Exception ex)
            {
                return OperationResult<Book>.Error($"An error occurred while updating the book: {ex.Message}");
            }
        }

        public async Task<IOperationResult<bool>> Remove(int id, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidateId(id);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingBook = await _bookRepository.GetById(id, ct);
                if (existingBook == null)
                {
                    return OperationResult<bool>.NotFound($"Book with ID {id} not found");
                }

                await _bookRepository.Remove(existingBook, ct);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Error($"An error occurred while removing the book: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByCategory(int categoryId, CancellationToken ct = default)
        {
            return await _bookRepository.GetBooksByCategory(categoryId, ct);
        }

        public async Task<IEnumerable<Book>> Search(string bookName, CancellationToken ct = default)
        {
            return await _bookRepository.Search(c => c.Name.Contains(bookName), ct);
        }

        public async Task<IEnumerable<Book>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default)
        {
            return await _bookRepository.SearchBookWithCategory(searchedValue, ct);
        }

        #region Private Validation Methods

        private static OperationResult<Book> ValidateBook(Book? book)
        {
            var nullCheck = ValidationHelper.ValidateNotNull<Book>(book, "Book");
            if (!nullCheck.Success) return nullCheck;

            return ValidationHelper.ValidateRequiredString<Book>(book!.Name, "Book name");
        }

        private static OperationResult<Book> ValidateBookForUpdate(Book? book)
        {
            var basicValidation = ValidateBook(book);
            if (!basicValidation.Success) return basicValidation;

            return ValidationHelper.ValidateId<Book>(book!.Id, "book");
        }

        private static OperationResult<bool> ValidateId(int id) =>
            ValidationHelper.ValidateIdForRemoval(id, "book");

        #endregion
    }
}
