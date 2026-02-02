using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookService _bookService;

        public CategoryService(ICategoryRepository categoryRepository, IBookService bookService)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }

        public async Task<IEnumerable<Category>> GetAll(CancellationToken ct = default)
        {
            return await _categoryRepository.GetAll(ct);
        }

        public async Task<PagedResponse<Category>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            return await _categoryRepository.GetAllWithPagination(pageNumber, pageSize, ct);
        }

        public async Task<Category?> GetById(int id, CancellationToken ct = default)
        {
            return await _categoryRepository.GetById(id, ct);
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

                var existingCategories = await _categoryRepository.Search(c => c.Name == category.Name, ct);
                if (existingCategories.Any())
                {
                    return OperationResult<Category>.Duplicate("This category name is already being used");
                }

                await _categoryRepository.Add(category, ct);

                return OperationResult<Category>.Ok(category);
            }
            catch (Exception ex)
            {
                return OperationResult<Category>.Error($"An error occurred while adding the category: {ex.Message}");
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
                    return OperationResult<Category>.NotFound($"Category with ID {category.Id} not found");
                }

                var duplicateCategories = await _categoryRepository.Search(c => c.Name == category.Name && c.Id != category.Id, ct);
                if (duplicateCategories.Any())
                {
                    return OperationResult<Category>.Duplicate("This category name is already being used");
                }

                await _categoryRepository.Update(category, ct);

                return OperationResult<Category>.Ok(category);
            }
            catch (Exception ex)
            {
                return OperationResult<Category>.Error($"An error occurred while updating the category: {ex.Message}");
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
                    return OperationResult<bool>.NotFound($"Category with ID {id} not found");
                }

                var books = await _bookService.GetBooksByCategory(id, ct);
                if (books.Any())
                {
                    return OperationResult<bool>.HasDependencies("Cannot delete category with associated books");
                }

                await _categoryRepository.Remove(existingCategory, ct);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Error($"An error occurred while removing the category: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Category>> Search(string categoryName, CancellationToken ct = default)
        {
            return await _categoryRepository.Search(c => c.Name.Contains(categoryName), ct);
        }

        #region Private Validation Methods

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

        #endregion
    }
}
