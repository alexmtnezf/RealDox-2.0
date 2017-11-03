using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RealDox.Core.Interfaces;
using RealDox.Core.Models;
using RealDox.Core.Data;

namespace RealDox.Core.Data.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private string DefaultConnection = "Server=(localdb)\\mssqllocaldb;Database=RealDox;Trusted_Connection=True;MultipleActiveResultSets=true";
        private List<TodoItem> _toDoList;

        private readonly ToDoContext _context;
        public ToDoRepository(ToDoContext context)
        {
            //If we dont want to use DI to provide externally our DbContextOptions instance uncomment this lines below
            //Refer to: https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext
            //This is another example: https://docs.microsoft.com/en-us/ef/core/saving/transactions
            
            /*var options = new DbContextOptionsBuilder<ToDoContext>()
            .UseSqlServer(new SqlConnection(DefaultConnection))
            .Options;

            _context = new ToDoContext(options);*/
            _context = context;

            //InitializeData();
        }

        public IEnumerable<TodoItem> All
        {
            //get { return _toDoList; }
            get { return _context.TodoItems.ToList(); }
        }

        public bool DoesItemExist(string Id)
        {
            //return _toDoList.Any(item => item.Id == Id);
            return _context.TodoItems.Any(item => item.Id == Id);
        }

        public TodoItem FindById(string Id)
        {
            //return _toDoList.FirstOrDefault(item => item.Id == Id);
            return _context.TodoItems.Find(Id);
        }

        public void Insert(TodoItem item)
        {
            //_toDoList.Add(item);
            _context.TodoItems.Add(item);
        }

        public void Update(TodoItem item)
        {
            var TodoItem = this.FindById(item.Id);
            var index = _toDoList.IndexOf(TodoItem);
            _toDoList.RemoveAt(index);
            _toDoList.Insert(index, item);
        }

        public void Delete(string Id)
        {
            _toDoList.Remove(this.FindById(Id));
        }

        public List<TodoItem> Find(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                return _context.TodoItems.Where(x => x.Description.Contains(searchString) ||
                            x.Name.Contains(searchString) ||
                            x.Notes.Contains(searchString))
                            .OrderBy(x => x.Name)
                            .ToList();

            }
            return new List<TodoItem>();
        }

        /*private void InitializeData()
        {
            _toDoList = new List<TodoItem>();

            var todoItem1 = new TodoItem
            {
                Id = "6bb8a868-dba1-4f1a-93b7-24ebce87e243",
                Name = "Learn app development",
                Notes = "Attend Xamarin University",
                Done = true
            };

            var todoItem2 = new TodoItem
            {
                Id = "b94afb54-a1cb-4313-8af3-b7511551b33b",
                Name = "Develop apps",
                Notes = "Use Xamarin Studio/Visual Studio",
                Done = false
            };

            var todoItem3 = new TodoItem
            {
                Id = "ecfa6f80-3671-4911-aabe-63cc442c1ecf",
                Name = "Publish apps",
                Notes = "All app stores",
                Done = false,
            };

            _toDoList.Add(todoItem1);
            _toDoList.Add(todoItem2);
            _toDoList.Add(todoItem3);
            _context.AddRange(_toDoList);
            //Registering changes in database, using DbContext.ChangeTracking object
            //_context.EnsureAutoHistory();
            _context.SaveChanges();

        }*/
    }
}