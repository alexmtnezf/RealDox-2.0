using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using RealDox.Core.Models;

namespace RealDox.Core.Data
{
    public class ToDoContext : DbContext
    {
        public ToDoContext()
        { }

        public ToDoContext(DbContextOptions<ToDoContext> options)
            : base(options)
        {
        }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Author> Authors { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.EnableAutoHistory();
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string ConnectionString = @"Server=(localdb)\\mssqllocaldb;Database=RealDox-Test;Trusted_Connection=True;MultipleActiveResultSets=true";
                optionsBuilder.UseSqlServer(ConnectionString,
                optionns =>
                    {
                        //Refer to: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                        optionns.EnableRetryOnFailure();
                        optionns.MigrationsAssembly("RealDox.Api");
                    });
            }

        }

    }
}