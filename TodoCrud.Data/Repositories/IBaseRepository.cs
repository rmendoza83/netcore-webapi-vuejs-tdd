namespace TodoCrud.Data.Repositories;

using TodoCrud.Data.Models;

public interface IBaseRepository<T> where T : IEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
