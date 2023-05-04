namespace TodoCrud.Test.Tests.Business;

using TodoCrud.Data.Models;
using TodoCrud.Business.Services;

public class TodoServiceTest : BaseServiceTest<Todo>
{
    protected override TodoService GetSystemUnderTest() =>
        new TodoService();
}