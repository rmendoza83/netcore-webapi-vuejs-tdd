using AutoFixture;
using TodoCrud.Api.Extensions;
using TodoCrud.Data;
using TodoCrud.Data.Models;

namespace TodoCrud.Api;

public class Startup
{
    private IConfiguration Configuration { get; set; }
    private IWebHostEnvironment WebHostEnvironment { get; set; }

    public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        Configuration = configuration;
        WebHostEnvironment = webHostEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDatabaseContext();
        services.AddRepositories();
        services.AddServices();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext dbContext)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            dbContext.Database.EnsureCreated();
            SeedDatabase(dbContext);
        }
        else
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        if (env.IsDevelopment())
        {
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "TodoCrud.Client";
                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });
        }
    }

    private void SeedDatabase(DatabaseContext dbContext)
    {
        var todos = dbContext.Todos;

        // Check if the Todos table is already populated
        if (todos.Any())
        {
            todos.RemoveRange(todos.ToArray()); // Empty the data
            dbContext.SaveChanges();
        }

        var faker = new Fixture();
        var todoEntities = faker.Build<Todo>()
            .Without(x => x.Id) // Exclude the Id property from auto-generation
            .Do(x => x.Id = faker.Create<Guid>())
            //.With(x => x.Id, faker.Create<Guid>())
            .CreateMany(100)
            .ToArray();

        // Add the Todos to the database
        todos.AddRange(todoEntities);
        dbContext.SaveChanges();
    }
}
