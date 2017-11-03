using System.Linq;
using RealDox.Core.Data;
using RealDox.Core.Interfaces;
using RealDox.Core.Models;

namespace RealDox.Api.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly IToDoRepository _repository;
        private readonly ToDoContext _DbContext;


        public SeedDataService(IToDoRepository repository, ToDoContext context)
        {
            _repository = repository;
            _DbContext = context;
        }

        public void EnsureSeedData()
        {
            if (_DbContext.AllMigrationsApplied())
            {
                if (!_DbContext.TodoItems.Any())
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
                    _repository.Insert(todoItem1);
                    _repository.Insert(todoItem2);
                    _repository.Insert(todoItem3);

                    _DbContext.SaveChanges();

                }


            }

        }
    }
}