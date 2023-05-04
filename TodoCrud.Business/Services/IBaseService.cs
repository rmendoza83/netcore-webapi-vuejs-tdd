using TodoCrud.Data.Models;

namespace TodoCrud.Business.Services;

public interface IBaseService<TEntity> where TEntity: class, IEntity, new()
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task InsertAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
}
