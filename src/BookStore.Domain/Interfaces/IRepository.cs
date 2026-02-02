using System.Linq.Expressions;
using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task Add(TEntity entity, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAll(CancellationToken ct = default);
        Task<PagedResponse<TEntity>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<TEntity?> GetById(int id, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsNoTracking(int id, CancellationToken ct = default);
        Task Update(TEntity entity, CancellationToken ct = default);
        Task Remove(TEntity entity, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<int> SaveChanges(CancellationToken ct = default);
    }
}