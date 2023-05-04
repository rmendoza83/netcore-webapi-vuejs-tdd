namespace TodoCrud.Api.Extensions;

using TodoCrud.Business.Services;
using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;

public static class DataExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IBaseRepository<Todo>, TodoRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped<IBaseService<Todo>, TodoService>();

        return services;
    }
}
