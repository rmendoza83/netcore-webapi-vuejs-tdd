namespace TodoCrud.Data;

using Microsoft.EntityFrameworkCore;
using TodoCrud.Data.Models;

public class DatabaseContext : DbContext
{
    public DbSet<Todo> Todos { get; set; }

    public virtual void SetModified<T>(T entity)
    {
        if (entity != null)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=todo.db");
    }
}
