using BookStore.Domain.Constants;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.Domain.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BookService(IBookRepository bookRepository, ICategoryRepository categoryRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<IOperationResult<IEnumerable<Book>>> GetAll(CancellationToken ct = default)
        {
            try
            {
                var books = await _bookRepository.GetAll(ct);
                return OperationResult<IEnumerable<Book>>.Ok(books);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<Book>>.Error($"An error occurred while retrieving books: {ex.Message}");
            }
        }

        public async Task<IOperationResult<PagedResponse<Book>>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidatePagination(pageNumber, pageSize);
                if (!validation.Success)
                {
                    return validation;
                }

                var paginatedBooks = await _bookRepository.GetAllWithPagination(pageNumber, pageSize, ct);
                return OperationResult<PagedResponse<Book>>.Ok(paginatedBooks);
            }
            catch (Exception ex)
            {
                return OperationResult<PagedResponse<Book>>.Error($"An error occurred while retrieving paginated books: {ex.Message}");
            }
        }

        public async Task<IOperationResult<Book>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidationHelper.ValidateId<Book>(id, "book");
                if (!validation.Success)
                {
                    return validation;
                }

                var book = await _bookRepository.GetById(id, ct);
                if (book == null)
                {
                    return OperationResult<Book>.NotFound(string.Format(ErrorMessages.BookNotFound, id));
                }

                return OperationResult<Book>.Ok(book);
            }
            catch (Exception ex)
            {
                return OperationResult<Book>.Error($"An error occurred while retrieving book: {ex.Message}");
            }
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

                var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == book.CategoryId, ct);
                if (!categoryExists)
                {
                    return OperationResult<Book>.NotFound(string.Format(ErrorMessages.CategoryNotFound, book.CategoryId));
                }

                var bookExists = await _bookRepository.ExistsAsync(b => b.Name == book.Name, ct);
                if (bookExists)
                {
                    return OperationResult<Book>.Duplicate(ErrorMessages.BookDuplicate);
                }

                await _bookRepository.Add(book, ct);
                return OperationResult<Book>.Ok(book);
            }
            catch (Exception ex)
            {
                return OperationResult<Book>.Error(string.Format(ErrorMessages.BookAddError, ex.Message));
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
                    return OperationResult<Book>.NotFound(string.Format(ErrorMessages.BookNotFound, book.Id));
                }

                var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == book.CategoryId, ct);
                if (!categoryExists)
                {
                    return OperationResult<Book>.NotFound(string.Format(ErrorMessages.CategoryNotFound, book.CategoryId));
                }

                var duplicateExists = await _bookRepository.ExistsAsync(b => b.Name == book.Name && b.Id != book.Id, ct);
                if (duplicateExists)
                {
                    return OperationResult<Book>.Duplicate(ErrorMessages.BookDuplicateOnUpdate);
                }

                await _bookRepository.Update(book, ct);
                return OperationResult<Book>.Ok(book);
            }
            catch (Exception ex)
            {
                return OperationResult<Book>.Error(string.Format(ErrorMessages.BookUpdateError, ex.Message));
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
                    return OperationResult<bool>.NotFound(string.Format(ErrorMessages.BookNotFound, id));
                }

                await _bookRepository.Remove(existingBook, ct);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Error(string.Format(ErrorMessages.BookRemoveError, ex.Message));
            }
        }

        public async Task<IOperationResult<IEnumerable<Book>>> GetBooksByCategory(int categoryId, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidationHelper.ValidateId<Book>(categoryId, "category");
                if (!validation.Success)
                {
                    return OperationResult<IEnumerable<Book>>.ValidationError(validation.Message!);
                }

                var books = await _bookRepository.GetBooksByCategory(categoryId, ct);
                return OperationResult<IEnumerable<Book>>.Ok(books);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<Book>>.Error($"An error occurred while retrieving books by category: {ex.Message}");
            }
        }

        public async Task<IOperationResult<IEnumerable<Book>>> Search(string bookName, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidationHelper.ValidateRequiredString<Book>(bookName, "Search term");
                if (!validation.Success)
                {
                    return OperationResult<IEnumerable<Book>>.ValidationError(validation.Message!);
                }

                var books = await _bookRepository.Search(c => c.Name.Contains(bookName), ct);
                return OperationResult<IEnumerable<Book>>.Ok(books);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<Book>>.Error($"An error occurred while searching books: {ex.Message}");
            }
        }

        public async Task<IOperationResult<IEnumerable<Book>>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidationHelper.ValidateRequiredString<Book>(searchedValue, "Search term");
                if (!validation.Success)
                {
                    return OperationResult<IEnumerable<Book>>.ValidationError(validation.Message!);
                }

                var books = await _bookRepository.SearchBookWithCategory(searchedValue, ct);
                return OperationResult<IEnumerable<Book>>.Ok(books);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<Book>>.Error($"An error occurred while searching books with category: {ex.Message}");
            }
        }

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

        private static OperationResult<PagedResponse<Book>> ValidatePagination(int pageNumber, int pageSize) =>
            ValidationHelper.ValidatePagination<Book>(pageNumber, pageSize);
    }
}
