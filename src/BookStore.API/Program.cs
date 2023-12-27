using BookStore.API.Configuration;
using BookStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.ResolveSwaggerConfig();

builder.Services.AddCors();

builder.Services.ResolveDependencies();

var app = builder.Build();

app.ConfigureSwagger();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var databaseSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        databaseSeeder.SeedData();
    }
}

app.Run();
