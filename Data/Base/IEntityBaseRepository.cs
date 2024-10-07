using System.Linq.Expressions;

namespace UniSportUAQ_API.Data.Base
{
	public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
		Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter, int? startIndex = null, int? endIndex = null, params Expression<Func<T, object>>[] includeProperties);
		Task<T?> GetByIdAsync(string id); 
		Task<T?> GetByIdAsync(string id, params Expression<Func<T, object>>[] includes);
		Task<T?> AddAsync(T entity);
		Task<T> UpdateAsync(T entity);
		Task<bool> DeleteAsync(string id);
	}
}
