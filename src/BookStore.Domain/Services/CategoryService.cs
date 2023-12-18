using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookService _bookService;

        public CategoryService(ICategoryRepository categoryRepository, IBookService bookService)
        {
            _categoryRepository = categoryRepository;
            _bookService = bookService;
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
                if (_categoryRepository.Search(c => c.Name == category.Name).Result.Any())
                    return new OperationResult<Category>(category) { Success = false, Message = "This category name is already being used" };

                await _categoryRepository.Add(category);

                return new OperationResult<Category>(category);
            }
            catch (Exception ex)
            {
                return new OperationResult<Category>(category) { Success = false, Message = ex.Message };
            }
        }

        public async Task<IOperationResult<Category>> Update(Category category)
        {
            try
            {
                if (_categoryRepository.Search(c => c.Name == category.Name && c.Id != category.Id).Result.Any())
                    return new OperationResult<Category>(category) { Success = false, Message = "This category name is already being used" };

                await _categoryRepository.Update(category);

                return new OperationResult<Category>(category);
            }
            catch (Exception ex)
            {
                return new OperationResult<Category>(category) { Success = false, Message = ex.Message };
            }
        }

        public async Task<bool> Remove(Category category)
        {
            var books = await _bookService.GetBooksByCategory(category.Id);
            if (books.Any()) return false;

            await _categoryRepository.Remove(category);
            return true;
        }

        public async Task<IEnumerable<Category>> Search(string categoryName)
        {
            return await _categoryRepository.Search(c => c.Name.Contains(categoryName));
        }

        public void Dispose()
        {
            _categoryRepository?.Dispose();
        }        
    }
}