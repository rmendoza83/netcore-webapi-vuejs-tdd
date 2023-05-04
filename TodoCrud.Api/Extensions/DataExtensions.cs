namespace TodoCrud.Api.Extensions;

using TodoCrud.Business.Services;
using TodoCrud.Data;
using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;

public static class DataExtensions
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>();
        
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Todo>, TodoRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IBaseService<Todo>, TodoService>();

        return services;
    }
}
