using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RealDox.Api.Services;

namespace RealDox.Api.Middleware
{
    public static class MiddlewareExtensions
    {
        public static async void UseSeedData(this IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            //var anotherService = app.ApplicationServices.GetRequiredService<DbContextOptions<ToDoContext>>();
            serviceScope.ServiceProvider.GetService<ISeedDataService>().EnsureSeedData();
        }
    }
}