using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealDox.Api.Filters;
using RealDox.Api.Serialization;
using RealDox.Core.Data;
using RealDox.Core.Interfaces;
using RealDox.Core.Models;

namespace RealDox.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ValidateModel]
    public class TodoController : Controller
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly ToDoContext _DbContext;
        public TodoController(IToDoRepository toDoRepository, ToDoContext context)
        {
            _DbContext = context;
            _toDoRepository = toDoRepository;
        }

        /// <summary>
        /// Retrieves a list of items
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="200">Product listed</response>
        /// <response code="500">Oops! Can't get your items list right now</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<IDictionary<string, string>>), 200)]
        [ProducesResponseType(typeof(void), 500)]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult List()
        {
            return Ok(_toDoRepository.All);
        }

        /// <summary>
        /// Retrieves a specific Item by unique id
        /// </summary>
        /// <param name="id">Item id</param>
        /// <remarks>Awesomeness!</remarks>
        /// <returns>An existing TodoItem</returns>
        /// <response code="200">Item found</response>
        /// <response code="400">Item id couldn't be found</response>
        /// <response code="500">Oops! Can't found your item right now</response>

        [HttpGet("{id}", Name = "GetSingleItem")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public IActionResult GetSingle(string id)
        {
            TodoItem item = _toDoRepository.FindById(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "notes" : "xyzf",
        ///        "done": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item">Item object</param>
        /// <returns>A newly-created TodoItem</returns>
        /// <response code="201">Returns the newly-created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="409">If the item already exists</response>
        /// <response code="500">Oops! Can't create your item right now</response>

        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 409)]
        [ProducesResponseType(typeof(void), 500)]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult Create([FromBody] TodoItem item)
        {
            try
            {
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest(ErrorCode.TodoItemNameAndNotesRequired.ToString());
                }
                bool itemExists = _toDoRepository.DoesItemExist(item.Id);
                if (itemExists)
                {
                    return StatusCode(StatusCodes.Status409Conflict, ErrorCode.TodoItemIDInUse.ToString());
                }
                _toDoRepository.Insert(item);
                _DbContext.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.CouldNotCreateItem.ToString());
            }
            return CreatedAtRoute("GetSingleItem", new { id = item.Id }, item);
        }

        /// <summary>
        /// Updates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "notes" : "xyzf",
        ///        "done": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item">Item object</param>
        /// <response code="400">If the item is null or couldn't be found</response>
        /// <response code="500">Oops! Can't update your item right now</response>

        [HttpPut]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult Edit([FromBody] TodoItem item)
        {
            try
            {
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest(ErrorCode.TodoItemNameAndNotesRequired.ToString());
                }
                var existingItem = _toDoRepository.Find(item.Id);
                if (existingItem == null)
                {
                    return NotFound(ErrorCode.RecordNotFound.ToString());
                }
                _toDoRepository.Update(item);
            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.CouldNotUpdateItem.ToString());
            }
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param> 

        [HttpDelete("{id}")]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult Delete(string id)
        {
            try
            {
                var item = _toDoRepository.Find(id);
                if (item == null)
                {
                    return NotFound(ErrorCode.RecordNotFound.ToString());
                }
                _toDoRepository.Delete(id);
            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.CouldNotDeleteItem.ToString());
            }
            return new NoContentResult();
        }



        //private readonly TodoContext _context;

        /*public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
        
        [HttpPost]
public IActionResult Create([FromBody] TodoItem item)
{
    if (item == null)
    {
        return BadRequest();
    }

    _context.TodoItems.Add(item);
    _context.SaveChanges();

    return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
}

[HttpPut("{id}")]
public IActionResult Update(long id, [FromBody] TodoItem item)
{
    if (item == null || item.Id != id)
    {
        return BadRequest();
    }

    var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
    if (todo == null)
    {
        return NotFound();
    }

    todo.IsComplete = item.IsComplete;
    todo.Name = item.Name;

    _context.TodoItems.Update(todo);
    _context.SaveChanges();
    return new NoContentResult();
}

[HttpDelete("{id}")]
public IActionResult Delete(long id)
{
    var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
    if (todo == null)
    {
        return NotFound();
    }

    _context.TodoItems.Remove(todo);
    _context.SaveChanges();
    return new NoContentResult();
}
        */
    }
}