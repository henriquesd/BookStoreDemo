using BookStore.Domain.Constants;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public CategoryService(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }

        public async Task<IOperationResult<IEnumerable<Category>>> GetAll(CancellationToken ct = default)
        {
            try
            {
                var categories = await _categoryRepository.GetAll(ct);
                return OperationResult<IEnumerable<Category>>.Ok(categories);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<Category>>.Error($"An error occurred while retrieving categories: {ex.Message}");
            }
        }

        public async Task<IOperationResult<PagedResponse<Category>>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidatePagination(pageNumber, pageSize);
                if (!validation.Success)
                {
                    return validation;
                }

                var paginatedCategories = await _categoryRepository.GetAllWithPagination(pageNumber, pageSize, ct);
                return OperationResult<PagedResponse<Category>>.Ok(paginatedCategories);
            }
            catch (Exception ex)
            {
                return OperationResult<PagedResponse<Category>>.Error($"An error occurred while retrieving paginated categories: {ex.Message}");
            }
        }

        public async Task<IOperationResult<Category>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidationHelper.ValidateId<Category>(id, "category");
                if (!validation.Success)
                {
                    return validation;
                }

                var category = await _categoryRepository.GetById(id, ct);
                if (category == null)
                {
                    return OperationResult<Category>.NotFound(string.Format(ErrorMessages.CategoryNotFound, id));
                }

                return OperationResult<Category>.Ok(category);
            }
            catch (Exception ex)
            {
                return OperationResult<Category>.Error($"An error occurred while retrieving category: {ex.Message}");
            }
        }

        public async Task<IOperationResult<Category>> Add(Category category, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidateCategory(category);
                if (!validation.Success)
                {
                    return validation;
                }

                var categoryExists = await _categoryRepository.ExistsAsync(c => c.Name == category.Name, ct);
                if (categoryExists)
                {
                    return OperationResult<Category>.Duplicate(ErrorMessages.CategoryDuplicate);
                }

                await _categoryRepository.Add(category, ct);

                return OperationResult<Category>.Ok(category);
            }
            catch (Exception ex)
            {
                return OperationResult<Category>.Error(string.Format(ErrorMessages.CategoryAddError, ex.Message));
            }
        }

        public async Task<IOperationResult<Category>> Update(Category category, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidateCategoryForUpdate(category);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingCategory = await _categoryRepository.GetByIdAsNoTracking(category.Id, ct);
                if (existingCategory == null)
                {
                    return OperationResult<Category>.NotFound(string.Format(ErrorMessages.CategoryNotFound, category.Id));
                }

                var duplicateExists = await _categoryRepository.ExistsAsync(c => c.Name == category.Name && c.Id != category.Id, ct);
                if (duplicateExists)
                {
                    return OperationResult<Category>.Duplicate(ErrorMessages.CategoryDuplicate);
                }

                await _categoryRepository.Update(category, ct);

                return OperationResult<Category>.Ok(category);
            }
            catch (Exception ex)
            {
                return OperationResult<Category>.Error(string.Format(ErrorMessages.CategoryUpdateError, ex.Message));
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

                var existingCategory = await _categoryRepository.GetById(id, ct);
                if (existingCategory == null)
                {
                    return OperationResult<bool>.NotFound(string.Format(ErrorMessages.CategoryNotFound, id));
                }

                var hasBooks = await _bookRepository.ExistsAsync(b => b.CategoryId == id, ct);
                if (hasBooks)
                {
                    return OperationResult<bool>.HasDependencies(ErrorMessages.CategoryHasDependencies);
                }

                await _categoryRepository.Remove(existingCategory, ct);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Error(string.Format(ErrorMessages.CategoryRemoveError, ex.Message));
            }
        }

        public async Task<IOperationResult<IEnumerable<Category>>> Search(string categoryName, CancellationToken ct = default)
        {
            try
            {
                var validation = ValidationHelper.ValidateRequiredString<Category>(categoryName, "Search term");
                if (!validation.Success)
                {
                    return OperationResult<IEnumerable<Category>>.ValidationError(validation.Message!);
                }

                var categories = await _categoryRepository.Search(c => c.Name.Contains(categoryName), ct);
                return OperationResult<IEnumerable<Category>>.Ok(categories);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<Category>>.Error($"An error occurred while searching categories: {ex.Message}");
            }
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

        private static OperationResult<PagedResponse<Category>> ValidatePagination(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                return OperationResult<PagedResponse<Category>>.ValidationError("Page number must be greater than zero");
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                return OperationResult<PagedResponse<Category>>.ValidationError("Page size must be between 1 and 100");
            }

            return new OperationResult<PagedResponse<Category>>(true, null);
        }
    }
}
