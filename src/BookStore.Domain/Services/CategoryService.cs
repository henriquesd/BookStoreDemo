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

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<PagedResponse<Category>> GetAllWithPagination(int pageNumber, int pageSize)
        {
            return await _categoryRepository.GetAllWithPagination(pageNumber, pageSize);
        }

        public async Task<Category> GetById(int id)
        {
            return await _categoryRepository.GetById(id);
        }

        public async Task<IOperationResult<Category>> Add(Category category)
        {
            try
            {
                var validation = ValidateCategory(category);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingCategories = await _categoryRepository.Search(c => c.Name == category.Name);
                if (existingCategories.Any())
                {
                    return new OperationResult<Category>(false, "This category name is already being used");
                }

                await _categoryRepository.Add(category);

                return new OperationResult<Category>(category);
            }
            catch (Exception ex)
            {
                return new OperationResult<Category>(false, $"An error occurred while adding the category: {ex.Message}");
            }
        }

        public async Task<IOperationResult<Category>> Update(Category category)
        {
            try
            {
                var validation = ValidateCategoryForUpdate(category);
                if (!validation.Success)
                {
                    return validation;
                }

                var existingCategory = await _categoryRepository.GetByIdAsNoTracking(category.Id);
                if (existingCategory == null)
                {
                    return new OperationResult<Category>(false, $"Category with ID {category.Id} not found");
                }

                var duplicateCategories = await _categoryRepository.Search(c => c.Name == category.Name && c.Id != category.Id);
                if (duplicateCategories.Any())
                {
                    return new OperationResult<Category>(false, "This category name is already being used");
                }

                await _categoryRepository.Update(category);

                return new OperationResult<Category>(category);
            }
            catch (Exception ex)
            {
                return new OperationResult<Category>(false, $"An error occurred while updating the category: {ex.Message}");
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

                var existingCategory = await _categoryRepository.GetById(id);
                if (existingCategory == null)
                {
                    return new OperationResult<bool>(false, $"Category with ID {id} not found");
                }

                var books = await _bookService.GetBooksByCategory(id);
                if (books.Any())
                {
                    return new OperationResult<bool>(false, "Cannot delete category with associated books");
                }

                await _categoryRepository.Remove(existingCategory);
                return new OperationResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new OperationResult<bool>(false, $"An error occurred while removing the category: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Category>> Search(string categoryName)
        {
            return await _categoryRepository.Search(c => c.Name.Contains(categoryName));
        }

        #region Private Validation Methods

        private OperationResult<Category> ValidateCategory(Category category)
        {
            if (category == null)
            {
                return new OperationResult<Category>(false, "Category cannot be null");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return new OperationResult<Category>(false, "Category name is required");
            }

            return new OperationResult<Category>(true, null);
        }

        private OperationResult<Category> ValidateCategoryForUpdate(Category category)
        {
            var basicValidation = ValidateCategory(category);
            if (!basicValidation.Success)
            {
                return basicValidation;
            }

            if (category.Id <= 0)
            {
                return new OperationResult<Category>(false, "Invalid category ID");
            }

            return new OperationResult<Category>(true, null);
        }

        private OperationResult<bool> ValidateId(int id)
        {
            if (id <= 0)
            {
                return new OperationResult<bool>(false, "Invalid category ID");
            }

            return new OperationResult<bool>(true, null);
        }

        #endregion
    }
}