using BookStore.Domain.Constants;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BookStore.Domain.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<BookService> _logger;

        public BookService(
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            ILogger<BookService> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IOperationResult<IEnumerable<Book>>> GetAll(CancellationToken ct = default)
        {
            _logger.LogDebug("Retrieving all books");
            var books = await _bookRepository.GetAll(ct);
            _logger.LogInformation("Retrieved {BookCount} books", books.Count());
            return OperationResult<IEnumerable<Book>>.SuccessResult(books);
        }

        public async Task<IOperationResult<PagedResponse<Book>>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            _logger.LogDebug("Retrieving paginated books. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);

            var validation = ValidatePagination(pageNumber, pageSize);
            if (!validation.Success)
            {
                _logger.LogWarning("Pagination validation failed: {ValidationMessage}", validation.Message);
                return validation;
            }

            var paginatedBooks = await _bookRepository.GetAllWithPagination(pageNumber, pageSize, ct);
            _logger.LogInformation("Retrieved {RecordCount} books (page {PageNumber} of {TotalPages})",
                paginatedBooks.Data.Count, paginatedBooks.PageNumber, paginatedBooks.TotalPages);
            return OperationResult<PagedResponse<Book>>.SuccessResult(paginatedBooks);
        }

        public async Task<IOperationResult<Book>> GetById(int id, CancellationToken ct = default)
        {
            _logger.LogDebug("Retrieving book by ID: {BookId}", id);

            var validation = ValidationHelper.ValidateId<Book>(id, "book");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid book ID: {BookId}", id);
                return validation;
            }

            var book = await _bookRepository.GetById(id, ct);
            if (book == null)
            {
                _logger.LogWarning("Book not found. BookId: {BookId}", id);
                return OperationResult<Book>.NotFound(string.Format(ErrorMessages.BookNotFound, id));
            }

            _logger.LogDebug("Book retrieved successfully. BookId: {BookId}, BookName: {BookName}", book.Id, book.Name);
            return OperationResult<Book>.SuccessResult(book);
        }

        public async Task<IOperationResult<Book>> Add(Book book, CancellationToken ct = default)
        {
            _logger.LogInformation("Adding new book with name: {BookName}", book?.Name);

            var validation = ValidateBook(book);
            if (!validation.Success)
            {
                _logger.LogWarning("Book validation failed: {ValidationMessage}", validation.Message);
                return validation;
            }

            var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == book.CategoryId, ct);
            if (!categoryExists)
            {
                var errorMessage = string.Format(ErrorMessages.CategoryNotFound, book.CategoryId);
                _logger.LogWarning("Category not found for book. CategoryId: {CategoryId}", book.CategoryId);
                return OperationResult<Book>.NotFound(errorMessage);
            }

            var bookExists = await _bookRepository.ExistsAsync(b => b.Name == book.Name, ct);
            if (bookExists)
            {
                _logger.LogWarning("Duplicate book detected. BookName: {BookName}", book.Name);
                return OperationResult<Book>.Duplicate(ErrorMessages.BookDuplicate);
            }

            await _bookRepository.Add(book, ct);
            _logger.LogInformation("Book added successfully. BookId: {BookId}, BookName: {BookName}", book.Id, book.Name);
            return OperationResult<Book>.SuccessResult(book);
        }

        public async Task<IOperationResult<Book>> Update(Book book, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating book. BookId: {BookId}, BookName: {BookName}", book?.Id, book?.Name);

            var validation = ValidateBookForUpdate(book);
            if (!validation.Success)
            {
                _logger.LogWarning("Book update validation failed: {ValidationMessage}", validation.Message);
                return validation;
            }

            var existingBook = await _bookRepository.GetByIdAsNoTracking(book.Id, ct);
            if (existingBook == null)
            {
                _logger.LogWarning("Book not found for update. BookId: {BookId}", book.Id);
                return OperationResult<Book>.NotFound(string.Format(ErrorMessages.BookNotFound, book.Id));
            }

            var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == book.CategoryId, ct);
            if (!categoryExists)
            {
                _logger.LogWarning("Category not found for book update. CategoryId: {CategoryId}", book.CategoryId);
                return OperationResult<Book>.NotFound(string.Format(ErrorMessages.CategoryNotFound, book.CategoryId));
            }

            var duplicateExists = await _bookRepository.ExistsAsync(b => b.Name == book.Name && b.Id != book.Id, ct);
            if (duplicateExists)
            {
                _logger.LogWarning("Duplicate book name on update. BookName: {BookName}", book.Name);
                return OperationResult<Book>.Duplicate(ErrorMessages.BookDuplicateOnUpdate);
            }

            await _bookRepository.Update(book, ct);
            _logger.LogInformation("Book updated successfully. BookId: {BookId}, BookName: {BookName}", book.Id, book.Name);
            return OperationResult<Book>.SuccessResult(book);
        }

        public async Task<IOperationResult<bool>> Remove(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("Removing book. BookId: {BookId}", id);

            var validation = ValidateId(id);
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid book ID for removal: {BookId}", id);
                return validation;
            }

            var existingBook = await _bookRepository.GetById(id, ct);
            if (existingBook == null)
            {
                _logger.LogWarning("Book not found for removal. BookId: {BookId}", id);
                return OperationResult<bool>.NotFound(string.Format(ErrorMessages.BookNotFound, id));
            }

            await _bookRepository.Remove(existingBook, ct);
            _logger.LogInformation("Book removed successfully. BookId: {BookId}, BookName: {BookName}", id, existingBook.Name);
            return OperationResult<bool>.SuccessResult(true);
        }

        public async Task<IOperationResult<IEnumerable<Book>>> GetBooksByCategory(int categoryId, CancellationToken ct = default)
        {
            _logger.LogDebug("Retrieving books by category. CategoryId: {CategoryId}", categoryId);

            var validation = ValidationHelper.ValidateId<Book>(categoryId, "category");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid category ID: {CategoryId}", categoryId);
                return OperationResult<IEnumerable<Book>>.ValidationError(validation.Message!);
            }

            var books = await _bookRepository.GetBooksByCategory(categoryId, ct);
            _logger.LogInformation("Retrieved {BookCount} books for category {CategoryId}", books.Count(), categoryId);
            return OperationResult<IEnumerable<Book>>.SuccessResult(books);
        }

        public async Task<IOperationResult<IEnumerable<Book>>> Search(string bookName, CancellationToken ct = default)
        {
            _logger.LogDebug("Searching books. SearchTerm: {SearchTerm}", bookName);

            var validation = ValidationHelper.ValidateRequiredString<Book>(bookName, "Search term");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid search term");
                return OperationResult<IEnumerable<Book>>.ValidationError(validation.Message!);
            }

            var books = await _bookRepository.Search(c => c.Name.Contains(bookName), ct);
            _logger.LogInformation("Search returned {BookCount} books for term '{SearchTerm}'", books.Count(), bookName);
            return OperationResult<IEnumerable<Book>>.SuccessResult(books);
        }

        public async Task<IOperationResult<IEnumerable<Book>>> SearchBookWithCategory(string searchedValue, CancellationToken ct = default)
        {
            _logger.LogDebug("Searching books with category. SearchTerm: {SearchTerm}", searchedValue);

            var validation = ValidationHelper.ValidateRequiredString<Book>(searchedValue, "Search term");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid search term for category search");
                return OperationResult<IEnumerable<Book>>.ValidationError(validation.Message!);
            }

            var books = await _bookRepository.SearchBookWithCategory(searchedValue, ct);
            _logger.LogInformation("Category search returned {BookCount} books for term '{SearchTerm}'", books.Count(), searchedValue);
            return OperationResult<IEnumerable<Book>>.SuccessResult(books);
        }

        public async Task<IOperationResult<PagedResponse<Book>>> SearchWithPagination(string bookName, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            _logger.LogDebug("Searching books with pagination. SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}", bookName, pageNumber, pageSize);

            var validation = ValidationHelper.ValidateRequiredString<Book>(bookName, "Search term");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid search term");
                return OperationResult<PagedResponse<Book>>.ValidationError(validation.Message!);
            }

            var paginationValidation = ValidatePagination(pageNumber, pageSize);
            if (!paginationValidation.Success)
            {
                _logger.LogWarning("Pagination validation failed: {ValidationMessage}", paginationValidation.Message);
                return paginationValidation;
            }

            var paginatedBooks = await _bookRepository.SearchWithPagination(b => b.Name.Contains(bookName), pageNumber, pageSize, ct);
            _logger.LogInformation("Paginated search returned {RecordCount} books (page {PageNumber} of {TotalPages}) for term '{SearchTerm}'",
                paginatedBooks.Data.Count, paginatedBooks.PageNumber, paginatedBooks.TotalPages, bookName);
            return OperationResult<PagedResponse<Book>>.SuccessResult(paginatedBooks);
        }

        public async Task<IOperationResult<PagedResponse<Book>>> SearchBookWithCategoryPagination(string searchedValue, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            _logger.LogDebug("Searching books with category and pagination. SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                searchedValue, pageNumber, pageSize);

            var validation = ValidationHelper.ValidateRequiredString<Book>(searchedValue, "Search term");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid search term for category search");
                return OperationResult<PagedResponse<Book>>.ValidationError(validation.Message!);
            }

            var paginationValidation = ValidatePagination(pageNumber, pageSize);
            if (!paginationValidation.Success)
            {
                _logger.LogWarning("Pagination validation failed: {ValidationMessage}", paginationValidation.Message);
                return paginationValidation;
            }

            var paginatedBooks = await _bookRepository.SearchBookWithCategoryPagination(searchedValue, pageNumber, pageSize, ct);
            _logger.LogInformation("Paginated category search returned {RecordCount} books (page {PageNumber} of {TotalPages}) for term '{SearchTerm}'",
                paginatedBooks.Data.Count, paginatedBooks.PageNumber, paginatedBooks.TotalPages, searchedValue);
            return OperationResult<PagedResponse<Book>>.SuccessResult(paginatedBooks);
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
