using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
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

        #region Overriding methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Enable auto history functionality
            modelBuilder.EnableAutoHistory(null);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string ConnectionString = @"Server=(localdb)\\mssqllocaldb;Database=Test;Trusted_Connection=True;MultipleActiveResultSets=true";
                optionsBuilder.UseSqlServer(ConnectionString,
                optionns =>
                    {
                        //Refer to: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                        optionns.EnableRetryOnFailure();
                        optionns.MigrationsAssembly("RealDox.Api");
                    });
            }

        }
        
        public override int SaveChanges()
        {
            this.EnsureAutoHistory();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.EnsureAutoHistory();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.EnsureAutoHistory();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess,cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.EnsureAutoHistory();
            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

    }
}