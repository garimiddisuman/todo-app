using TodoApp.Core.Entities;
using TodoApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TodoApp.API.Controllers
{
  [ApiController]
  [Route("api/todos")]
  public class TodosController : ControllerBase
  {
    private readonly TodosService _service;
    public TodosController(TodosService service)
    {
      _service = service;
    }

    public class CreateTodoRequest
    {
      public required string TodoTitle { get; set; }
      public DateTime? DueDate { get; set; }
    }

    public class SetDueDateRequest
    {
      public DateTime? DueDate { get; set; }
    }

    public class AddTaskRequest
    {
      public required string Title { get; set; }
      public DateTime? DueDate { get; set; }
    }

    private long? GetUserIdFromCookie()
    {
      if (Request.Cookies.TryGetValue("userId", out var userIdStr) && long.TryParse(userIdStr, out var userId))
        return userId;
      return null;
    }

    [HttpGet]
    public ActionResult<List<Todo>> GetAll()
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      return Ok(_service.GetAll(userId.Value));
    }

    [HttpGet("{id}")]
    public ActionResult<Todo> Get(int id)
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      var todo = _service.GetById(id, userId.Value);
      if (todo == null) return NotFound();
      return Ok(todo);
    }

    [HttpPost]
    public ActionResult<Todo> Create([FromBody] CreateTodoRequest req)
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      var todo = _service.Add(req.TodoTitle, req.DueDate, userId.Value);
      return CreatedAtAction(nameof(Get), new { id = todo.TodoId }, todo);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      if (!_service.Delete(id, userId.Value)) return NotFound();
      return NoContent();
    }

    [HttpPatch("{id}/duedate")]
    public IActionResult SetDueDate(int id, [FromBody] SetDueDateRequest req)
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      if (!_service.SetDueDate(id, req.DueDate, userId.Value)) return NotFound();
      return Ok(_service.GetById(id, userId.Value));
    }

    // Add/Remove/Toggle tasks for a specific Todo
    [HttpPost("{todoId}/tasks")]
    public ActionResult AddTask(int todoId, [FromBody] AddTaskRequest req)
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      var todo = _service.GetById(todoId, userId.Value);
      if (todo == null) return NotFound();
      todo.AddTask(req.Title, req.DueDate);
      return Ok(todo);
    }

    [HttpDelete("{todoId}/tasks/{taskId}")]
    public IActionResult DeleteTask(int todoId, int taskId)
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      var todo = _service.GetById(todoId, userId.Value);
      if (todo == null) return NotFound();
      if (!todo.DeleteTask(taskId)) return NotFound();
      return NoContent();
    }

    [HttpPost("{todoId}/tasks/{taskId}/toggle")]
    public IActionResult ToggleTask(int todoId, int taskId)
    {
      var userId = GetUserIdFromCookie();
      if (userId == null) return Unauthorized();
      var todo = _service.GetById(todoId, userId.Value);
      if (todo == null) return NotFound();
      if (!todo.ToggleTask(taskId)) return NotFound();
      return Ok(todo);
    }
  }
}
