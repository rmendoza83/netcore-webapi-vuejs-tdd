namespace TodoCrud.Test.Tests.Api;

using Microsoft.Extensions.Logging;
using TodoCrud.Api.Controllers;
using TodoCrud.Business.Services;
using TodoCrud.Data.Models;

public class TodoControllerTest : BaseControllerTest<Todo>
{
    protected override TodoController GetSystemUnderTest(IBaseService<Todo> service, ILogger<IBaseService<Todo>> logger) =>
        new TodoController(service, logger);
}
