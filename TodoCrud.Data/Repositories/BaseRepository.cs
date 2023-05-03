namespace TodoCrud.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using TodoCrud.Data.Models;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity, new()
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task InsertAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.SetModified(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}