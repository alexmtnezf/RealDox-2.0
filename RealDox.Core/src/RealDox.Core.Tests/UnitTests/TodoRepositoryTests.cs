using System.Linq;
using Microsoft.EntityFrameworkCore;
using RealDox.Core.Data;
using RealDox.Core.Data.Repositories;
using RealDox.Core.Models;
using Xunit;

namespace RealDox.Core.Tests
{
    
    public class TodoRepositoryTests
    {
        [Fact]
        public void Add_writes_to_database()
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            // Run the test against one instance of the context
            using (var context = new ToDoContext(options))
            {
                var repo = new ToDoRepository(context);
                repo.Insert(new TodoItem{
                    Name = "Item1",
                    Notes = "Notes1",
                    Done = false,
                    Description = "Description of item 1"

                });
                context.SaveChanges();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new ToDoContext(options))
            {
                Assert.Equal(1, context.TodoItems.Count());
                Assert.Equal("Item1", context.TodoItems.Single().Name);
            }
        }

        [Theory]
        [InlineData("Notes")]
        [InlineData("Pepito")]
        public void Find_searches_items(string value)
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new ToDoContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var repo = new ToDoRepository(context);
                TodoItem item1 = new TodoItem{
                    Name = "Pepito 2",
                    Notes = "Notes about Pepito2",
                    Done = false,
                    Description = "Description of Pepito 2"

                };
                TodoItem item2 = new TodoItem{
                    Name = "Item2",
                    Notes = "Notes2",
                    Done = true,
                    Description = "Description of Item2"

                };
                TodoItem item3 = new TodoItem{
                    Name = "Pepito",
                    Notes = "Notes3",
                    Done = false,
                    Description = "Description of Pepito"

                };

                repo.Insert(item1);
                repo.Insert(item2);
                repo.Insert(item3);
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new ToDoContext(options))
            {
                //context.Database.EnsureDeleted();
                var repo = new ToDoRepository(context);
                var result = repo.Find(value);
                if(value.Equals("Pepito"))
                    Assert.Equal(2 , result.Count);
                else
                    Assert.Equal(3 , result.Count);
            }
        }

        
    }
}
