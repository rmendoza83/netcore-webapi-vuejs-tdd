namespace TodoCrud.Test.Tests.Data;

using TodoCrud.Data;
using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;

public class TodoRepositoryTest : BaseRepositoryTest<Todo>
{
    protected override BaseRepository<Todo> GetSystemUnderTest(DatabaseContext dbContext) =>
        new TodoRepository(dbContext);
}
