using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Services;
using TodoApp.Core.Entities;

namespace TodoApp.API.Controllers
{
  [ApiController]
  [Route("api/users")]
  public class UsersController : ControllerBase
  {
    private readonly UsersService _service;
    public UsersController(UsersService service)
    {
      _service = service;
    }

    [HttpPost("login")]
    public ActionResult<User> Login([FromBody] LoginRequest req)
    {
      long userId = req.UserId ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      var user = _service.GetByName(req.Name) ?? _service.Add(req.Name, userId);
      Response.Cookies.Append("userId", user.UserId.ToString(), new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Lax });
      return Ok(user);
    }

    [HttpGet]
    public ActionResult<List<User>> GetAll()
    {
      return Ok(_service.GetAll());
    }

    [HttpGet("me/todos")]
    public ActionResult<List<Todo>> GetMyTodos()
    {
      if (!Request.Cookies.TryGetValue("userId", out var userIdStr) || !long.TryParse(userIdStr, out var userId))
        return Unauthorized();
      var user = _service.GetAll().FirstOrDefault(u => u.UserId == userId);
      if (user == null) return Ok(new List<Todo>()); // Return empty array for new user
      return Ok(user.Todos ?? new List<Todo>());
    }

    [HttpDelete("{userId}")]
    public IActionResult Delete(long userId)
    {
      if (!_service.Delete(userId)) return NotFound();
      return NoContent();
    }

    public class LoginRequest
    {
      public required string Name { get; set; }
      public long? UserId { get; set; }
    }
  }
}
