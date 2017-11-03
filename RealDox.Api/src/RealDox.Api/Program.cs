using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealDox.Core.Data;
using RealDox.Core.Interfaces;
using RealDox.Core.Models;

namespace RealDox.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                try
                {
                    InitializeDatabaseAsync(serviceProvider).Wait();
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB");
                }
            }
            host.Run();

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000", "http://*:5001")
                .UseStartup<Startup>()
                .Build();

        
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceScope)
        {
            await GetTestAuthors(serviceScope);
        }

        public static async Task GetTestAuthors(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<ToDoContext>();
            var repo = serviceProvider.GetService<IAuthorRepository>();
            var list = await repo.ListAsync();
            if (list.Count == 0)
            {
                
                await repo.AddAsync(new Author()
                {
                    
                    FullName = "Steve Smith",
                    TwitterAlias = "ardalis"
                });
                await repo.AddAsync(new Author()
                {
                    
                    FullName = "Neil Gaiman",
                    TwitterAlias = "neilhimself"
                });

                dbContext.SaveChanges();
            }


            /*var authors = dbContext.Authors;
            foreach (var author in authors)
            {
                dbContext.Remove(author);
            }
            dbContext.SaveChanges();
            dbContext.Authors.Add(new Author()
            {
                Id=1,
                FullName = "Steve Smith",
                TwitterAlias = "ardalis"
            });
            dbContext.Authors.Add(new Author()
            {
                Id=2,
                FullName = "Neil Gaiman",
                TwitterAlias = "neilhimself"
            });*/
            //dbContext.SaveChanges();
        }

    }
}
