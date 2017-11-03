using System.Linq;
using RealDox.Core.Models;

namespace RealDox.Core.Data
{
    public static class ToDoContextExtensions
    {
        public static void EnsureSeedData(this ToDoContext context)
        {
            if (context.AllMigrationsApplied())
            {
                if (!context.TodoItems.Any())
            {
                var todoItem1 = new TodoItem
                {
                    //Id = "6bb8a868-dba1-4f1a-93b7-24ebce87e243",
                    Name = "Learn app development",
                    Notes = "Attend Xamarin University",
                    Done = true
                };

                var todoItem2 = new TodoItem
                {
                    //Id = "b94afb54-a1cb-4313-8af3-b7511551b33b",
                    Name = "Develop apps",
                    Notes = "Use Xamarin Studio/Visual Studio",
                    Done = false
                };

                var todoItem3 = new TodoItem
                {
                    //Id = "ecfa6f80-3671-4911-aabe-63cc442c1ecf",
                    Name = "Publish apps",
                    Notes = "All app stores",
                    Done = false,
                };
                context.TodoItems.Add(todoItem1);
                context.TodoItems.Add(todoItem2);
                context.TodoItems.Add(todoItem3);

                context.SaveChanges();

            }

            }
        }
        
    }
}