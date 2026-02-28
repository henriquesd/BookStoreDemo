using System.Linq.Expressions;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly BookStoreDbContext Db;

        protected readonly DbSet<TEntity> DbSet;

        protected Repository(BookStoreDbContext db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }

        public virtual async Task Add(TEntity entity, CancellationToken ct = default)
        {
            DbSet.Add(entity);
            await SaveChanges(ct);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll(CancellationToken ct = default)
        {
            return await DbSet.AsNoTracking().ToListAsync(ct);
        }

        public virtual async Task<PagedResponse<TEntity>> GetAllWithPagination(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var totalRecords = await Db.Set<TEntity>().AsNoTracking().CountAsync(ct);

            var entities = await Db.Set<TEntity>()
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var pagedResponse = new PagedResponse<TEntity>(entities, pageNumber, pageSize, totalRecords);

            return pagedResponse;
        }

        public virtual async Task<TEntity?> GetById(int id, CancellationToken ct = default)
        {
            return await DbSet.FindAsync([id], ct);
        }

        public virtual async Task<TEntity?> GetByIdAsNoTracking(int id, CancellationToken ct = default)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public virtual async Task Update(TEntity entity, CancellationToken ct = default)
        {
            DbSet.Update(entity);
            await SaveChanges(ct);
        }

        public virtual async Task Remove(TEntity entity, CancellationToken ct = default)
        {
            DbSet.Remove(entity);
            await SaveChanges(ct);
        }

        public async Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public virtual async Task<PagedResponse<TEntity>> SearchWithPagination(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = DbSet.AsNoTracking().Where(predicate);
            var totalRecords = await query.CountAsync(ct);

            var entities = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResponse<TEntity>(entities, pageNumber, pageSize, totalRecords);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<int> SaveChanges(CancellationToken ct = default)
        {
            return await Db.SaveChangesAsync(ct);
        }
    }
}
