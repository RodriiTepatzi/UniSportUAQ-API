using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UniSportUAQ_API.Data.Base
{
	public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
	{
		private readonly AppDbContext _context;

		public EntityBaseRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<T?> AddAsync(T entity)
		{
			try
			{
				var result = await _context.Set<T>().AddAsync(entity);
				await _context.SaveChangesAsync();

				return result.Entity;
			}
			catch
			{
				return null;
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

		public async Task<T?> GetByIdAsync(string id) => await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);

		public async Task<T?> GetByIdAsync(string id, params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>();

			if (includes != null)
			{
				query = includes.Aggregate(query, (current, include) => current.Include(include));
			}

			return await query.FirstOrDefaultAsync(entity => EF.Property<string>(entity, "Id") == id);
		}

		public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
		{
			IQueryable<T> query = _context.Set<T>();
			query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

			return await query.ToListAsync();
		}

		public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
		{
			IQueryable<T> query = _context.Set<T>();

			// Apply filter if provided
			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (includeProperties != null)
			{
				query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
			}

			return await query.ToListAsync();
		}

		public async Task<T> UpdateAsync(T entity)
		{
			EntityEntry entityEntry = _context.Entry<T>(entity);
			entityEntry.State = EntityState.Modified;

			await _context.SaveChangesAsync();

			return entity;
		}

		public async Task<bool> DeleteAsync(string id)
		{
			var entity = await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);

			if (entity is not null)
			{
				EntityEntry entityEntry = _context.Entry<T>(entity);
				entityEntry.State = EntityState.Deleted;

				await _context.SaveChangesAsync();

				return true;
			}

			return false;
		}
	}
}
