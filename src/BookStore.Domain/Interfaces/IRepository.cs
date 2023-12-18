using System.Linq.Expressions;
using BookStore.Domain.Models;
using static BookStore.Domain.Models.Pagination;

namespace BookStore.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task Add(TEntity entity);
        Task<List<TEntity>> GetAll();
        Task<PagedResponse<TEntity>> GetAllWithPagination(int pageNumber, int pageSize);
        Task<TEntity> GetById(int id);
        Task Update(TEntity entity);
        Task Remove(TEntity entity);
        Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate);
        Task<int> SaveChanges();
    }
}