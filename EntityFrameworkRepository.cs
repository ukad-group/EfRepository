using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data
{
    internal class EntityFrameworkRepository<T> : IRepository<T> where T : class
    {
        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IRepository{T}"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">unitOfWork</exception>
        public EntityFrameworkRepository(IEfRepositoryDbContext dbContext)
        {
            DbContext = dbContext as DbContext ?? throw new ArgumentNullException("dbContext");

            DbSet = DbContext.Set<T>();
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        /// <summary>
        /// ToListAsync
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async Task<IList<TSource>> ToListAsync<TSource>(IQueryable<TSource> source, CancellationToken ct)
        {
            return await source.ToListAsync(ct);
        }

        /// <summary>
        /// FirstOrDefaultAsync
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken ct)
        {
            return await source.FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Gets all as no tracking.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllAsNoTracking()
        {
            return DbSet.AsNoTracking();
        }

        /// <summary>
        /// Counts asynchronously.
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(IQueryable<T> source, CancellationToken ct)
        {
            return await source.CountAsync(ct);
        }

        /// <summary>
        /// Includes the specified x function.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="xFunc">The x function.</param>
        /// <returns></returns>
        public virtual IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> xFunc)
        {
            return DbSet.Include(xFunc);
        }

        /// <summary>
        /// Include
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="xFunc"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Include<TProperty>(IQueryable<T> queryable, Expression<Func<T, TProperty>> xFunc)
        {
            return queryable.Include(xFunc);
        }

        /// <summary>
        /// ThenInclude
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="T2Property"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="xFunc"></param>
        /// <returns></returns>
        public virtual IIncludableQueryable<T, T2Property> ThenInclude<TProperty, T2Property>(
            IIncludableQueryable<T, TProperty> queryable, Expression<Func<TProperty, T2Property>> xFunc)
        {
            return queryable.ThenInclude(xFunc);
        }

        /// <summary>
        /// GetByIdAsync
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(int id, CancellationToken ct)
        {
            return await DbSet.FindAsync(keyValues: new object[] { id }, cancellationToken: ct);
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Add(T entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        /// <summary>
        /// AddAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(T entity, CancellationToken ct)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                await DbSet.AddAsync(entity, ct);
            }
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Update(T entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
            //return dbEntityEntry;
        }


        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(T entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return DbSet.Find(keyValues: new object[] { id });
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public virtual void Delete(int id)
        {
            while (true)
            {
                var entity = GetById(id);
                if (entity == null) return; // not found; assume already deleted.
            }
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        /// <summary>
        /// SaveChangesAsync
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await DbContext.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Adds the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void AddRange(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
        }

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

    }
}
