using BookStore.Domain.Constants;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BookStore.Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IBookRepository bookRepository,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IOperationResult<IEnumerable<Category>>> GetAll(CancellationToken ct = default)
        {
            _logger.LogDebug("Retrieving all categories");
            var categories = await _categoryRepository.GetAll(ct);
            _logger.LogInformation("Retrieved {CategoryCount} categories", categories.Count());
            return OperationResult<IEnumerable<Category>>.SuccessResult(categories);
        }

        public async Task<IOperationResult<PagedResponse<Category>>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            _logger.LogDebug("Retrieving paginated categories. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);

            var validation = ValidatePagination(pageNumber, pageSize);
            if (!validation.Success)
            {
                _logger.LogWarning("Pagination validation failed: {ValidationMessage}", validation.Message);
                return validation;
            }

            var paginatedCategories = await _categoryRepository.GetAllWithPagination(pageNumber, pageSize, ct);
            _logger.LogInformation("Retrieved {RecordCount} categories (page {PageNumber} of {TotalPages})",
                paginatedCategories.Data.Count, paginatedCategories.PageNumber, paginatedCategories.TotalPages);
            return OperationResult<PagedResponse<Category>>.SuccessResult(paginatedCategories);
        }

        public async Task<IOperationResult<Category>> GetById(int id, CancellationToken ct = default)
        {
            _logger.LogDebug("Retrieving category by ID: {CategoryId}", id);

            var validation = ValidationHelper.ValidateId<Category>(id, "category");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid category ID: {CategoryId}", id);
                return validation;
            }

            var category = await _categoryRepository.GetById(id, ct);
            if (category == null)
            {
                _logger.LogWarning("Category not found. CategoryId: {CategoryId}", id);
                return OperationResult<Category>.NotFound(string.Format(ErrorMessages.CategoryNotFound, id));
            }

            _logger.LogDebug("Category retrieved successfully. CategoryId: {CategoryId}, CategoryName: {CategoryName}", category.Id, category.Name);
            return OperationResult<Category>.SuccessResult(category);
        }

        public async Task<IOperationResult<Category>> Add(Category category, CancellationToken ct = default)
        {
            _logger.LogInformation("Adding new category with name: {CategoryName}", category?.Name);

            var validation = ValidateCategory(category);
            if (!validation.Success)
            {
                _logger.LogWarning("Category validation failed: {ValidationMessage}", validation.Message);
                return validation;
            }

            var categoryExists = await _categoryRepository.ExistsAsync(c => c.Name == category.Name, ct);
            if (categoryExists)
            {
                _logger.LogWarning("Duplicate category detected. CategoryName: {CategoryName}", category.Name);
                return OperationResult<Category>.Duplicate(ErrorMessages.CategoryDuplicate);
            }

            await _categoryRepository.Add(category, ct);
            _logger.LogInformation("Category added successfully. CategoryId: {CategoryId}, CategoryName: {CategoryName}", category.Id, category.Name);
            return OperationResult<Category>.SuccessResult(category);
        }

        public async Task<IOperationResult<Category>> Update(Category category, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating category. CategoryId: {CategoryId}, CategoryName: {CategoryName}", category?.Id, category?.Name);

            var validation = ValidateCategoryForUpdate(category);
            if (!validation.Success)
            {
                _logger.LogWarning("Category update validation failed: {ValidationMessage}", validation.Message);
                return validation;
            }

            var existingCategory = await _categoryRepository.GetByIdAsNoTracking(category.Id, ct);
            if (existingCategory == null)
            {
                _logger.LogWarning("Category not found for update. CategoryId: {CategoryId}", category.Id);
                return OperationResult<Category>.NotFound(string.Format(ErrorMessages.CategoryNotFound, category.Id));
            }

            var duplicateExists = await _categoryRepository.ExistsAsync(c => c.Name == category.Name && c.Id != category.Id, ct);
            if (duplicateExists)
            {
                _logger.LogWarning("Duplicate category name on update. CategoryName: {CategoryName}", category.Name);
                return OperationResult<Category>.Duplicate(ErrorMessages.CategoryDuplicate);
            }

            await _categoryRepository.Update(category, ct);
            _logger.LogInformation("Category updated successfully. CategoryId: {CategoryId}, CategoryName: {CategoryName}", category.Id, category.Name);
            return OperationResult<Category>.SuccessResult(category);
        }

        public async Task<IOperationResult<bool>> Remove(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("Removing category. CategoryId: {CategoryId}", id);

            var validation = ValidateId(id);
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid category ID for removal: {CategoryId}", id);
                return validation;
            }

            var existingCategory = await _categoryRepository.GetById(id, ct);
            if (existingCategory == null)
            {
                _logger.LogWarning("Category not found for removal. CategoryId: {CategoryId}", id);
                return OperationResult<bool>.NotFound(string.Format(ErrorMessages.CategoryNotFound, id));
            }

            var hasBooks = await _bookRepository.ExistsAsync(b => b.CategoryId == id, ct);
            if (hasBooks)
            {
                _logger.LogWarning("Cannot remove category with dependencies. CategoryId: {CategoryId}", id);
                return OperationResult<bool>.HasDependencies(ErrorMessages.CategoryHasDependencies);
            }

            await _categoryRepository.Remove(existingCategory, ct);
            _logger.LogInformation("Category removed successfully. CategoryId: {CategoryId}, CategoryName: {CategoryName}", id, existingCategory.Name);
            return OperationResult<bool>.SuccessResult(true);
        }

        public async Task<IOperationResult<IEnumerable<Category>>> Search(string categoryName, CancellationToken ct = default)
        {
            _logger.LogDebug("Searching categories. SearchTerm: {SearchTerm}", categoryName);

            var validation = ValidationHelper.ValidateRequiredString<Category>(categoryName, "Search term");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid search term for category search");
                return OperationResult<IEnumerable<Category>>.ValidationError(validation.Message!);
            }

            var categories = await _categoryRepository.Search(c => c.Name.Contains(categoryName), ct);
            _logger.LogInformation("Search returned {CategoryCount} categories for term '{SearchTerm}'", categories.Count(), categoryName);
            return OperationResult<IEnumerable<Category>>.SuccessResult(categories);
        }

        public async Task<IOperationResult<PagedResponse<Category>>> SearchWithPagination(string categoryName, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            _logger.LogDebug("Searching categories with pagination. SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                categoryName, pageNumber, pageSize);

            var validation = ValidationHelper.ValidateRequiredString<Category>(categoryName, "Search term");
            if (!validation.Success)
            {
                _logger.LogWarning("Invalid search term for category search");
                return OperationResult<PagedResponse<Category>>.ValidationError(validation.Message!);
            }

            var paginationValidation = ValidatePagination(pageNumber, pageSize);
            if (!paginationValidation.Success)
            {
                _logger.LogWarning("Pagination validation failed: {ValidationMessage}", paginationValidation.Message);
                return paginationValidation;
            }

            var paginatedCategories = await _categoryRepository.SearchWithPagination(c => c.Name.Contains(categoryName), pageNumber, pageSize, ct);
            _logger.LogInformation("Paginated search returned {RecordCount} categories (page {PageNumber} of {TotalPages}) for term '{SearchTerm}'",
                paginatedCategories.Data.Count, paginatedCategories.PageNumber, paginatedCategories.TotalPages, categoryName);
            return OperationResult<PagedResponse<Category>>.SuccessResult(paginatedCategories);
        }

        private static OperationResult<Category> ValidateCategory(Category? category)
        {
            var nullCheck = ValidationHelper.ValidateNotNull<Category>(category, "Category");
            if (!nullCheck.Success) return nullCheck;

            return ValidationHelper.ValidateRequiredString<Category>(category!.Name, "Category name");
        }

        private static OperationResult<Category> ValidateCategoryForUpdate(Category? category)
        {
            var basicValidation = ValidateCategory(category);
            if (!basicValidation.Success) return basicValidation;

            return ValidationHelper.ValidateId<Category>(category!.Id, "category");
        }

        private static OperationResult<bool> ValidateId(int id) =>
            ValidationHelper.ValidateIdForRemoval(id, "category");

        private static OperationResult<PagedResponse<Category>> ValidatePagination(int pageNumber, int pageSize) =>
            ValidationHelper.ValidatePagination<Category>(pageNumber, pageSize);
    }
}
