namespace TodoCrud.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TodoCrud.Business.Services;
using TodoCrud.Data.Models;

[Route("api/[controller]")]
[ApiController]
public class TodoController : BaseController<Todo>
{
    public TodoController(IBaseService<Todo> service, ILogger<IBaseService<Todo>> logger) : base(service, logger) { }
}
