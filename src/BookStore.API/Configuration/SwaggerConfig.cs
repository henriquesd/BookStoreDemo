using BookStore.Infrastructure.Context;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace BookStore.API.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection ResolveSwaggerConfig(this IServiceCollection services)
        {
            services.AddScoped<BookStoreDbContext>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "BookStore API",
                    Version = "v1"
                });
            });

            services.ConfigureSwaggerGen(options => options.ExampleFilters());
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

            return services;
        }

        public static WebApplication ConfigureSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

            return app;
        }
    }
}