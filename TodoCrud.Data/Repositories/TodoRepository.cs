namespace TodoCrud.Data.Repositories;

using TodoCrud.Data.Models;

public class TodoRepository : BaseRepository<Todo>
{
    public TodoRepository(DatabaseContext dbContext) : base(dbContext) { }
}
