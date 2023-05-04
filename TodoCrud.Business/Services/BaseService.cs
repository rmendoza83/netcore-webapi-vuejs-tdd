namespace TodoCrud.Business.Services;

using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;

public abstract class BaseService<TEntity> : IBaseService<TEntity> 
    where TEntity : class, IEntity, new()
{
    private IBaseRepository<TEntity>? _repository;

    public IBaseRepository<TEntity> Repository
    {
        get
        {
            return _repository!;
        }
        set
        {
            _repository = value;
        }
    }

    public BaseService() { }

    public BaseService(IBaseRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Repository.GetAllAsync();
    }

    public async Task InsertAsync(TEntity entity)
    {
        await Repository.InsertAsync(entity);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        await Repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await Repository.DeleteAsync(id);
    }
}