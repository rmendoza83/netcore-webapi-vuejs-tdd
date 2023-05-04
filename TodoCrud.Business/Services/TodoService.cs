namespace TodoCrud.Business.Services;

using Microsoft.Extensions.DependencyInjection;
using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;

public class TodoService : BaseService<Todo>
{
    public TodoService() { }

    [ActivatorUtilitiesConstructor]
    public TodoService(TodoRepository repository) : base(repository) { }
}
