using System.Collections.Generic;
using RealDox.Core.Models;

namespace RealDox.Core.Interfaces
{
    public interface IToDoRepository
    {
        bool DoesItemExist(string id);
        IEnumerable<TodoItem> All { get; }
        TodoItem FindById(string id);
        List<TodoItem> Find(string searchString);
        void Insert(TodoItem item);
        void Update(TodoItem item);
        void Delete(string id);
    }
}