using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Core.Entities;
using Xunit;

namespace TodoApp.test.API.Controllers
{
  public class UsersControllerTests : IClassFixture<WebApplicationFactory<TodoApp.Program>>
  {
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<TodoApp.Program> factory)
    {
      _client = factory.CreateClient();
    }

    [Fact]
    public async Task Can_Create_And_Get_User()
    {
      var req = new { Name = "TestUser", UserId = 123456789L };
      var response = await _client.PostAsJsonAsync("/api/users/login", req);
      response.EnsureSuccessStatusCode();
      var user = await response.Content.ReadFromJsonAsync<User>();
      Assert.NotNull(user);
      Assert.Equal("TestUser", user.Name);
      Assert.Equal(123456789L, user.UserId);
    }

    [Fact]
    public async Task GetAll_Returns_Users()
    {
      // Login to get cookie
      var req = new { Name = "TestUser", UserId = 123456789L };
      var login = await _client.PostAsJsonAsync("/api/users/login", req);
      login.EnsureSuccessStatusCode();
      var cookie = login.Headers.GetValues("Set-Cookie");
      _client.DefaultRequestHeaders.Remove("Cookie");
      _client.DefaultRequestHeaders.Add("Cookie", string.Join("; ", cookie));
      var response = await _client.GetAsync("/api/users");
      response.EnsureSuccessStatusCode();
      var users = await response.Content.ReadFromJsonAsync<List<User>>();
      Assert.NotNull(users);
    }

    [Fact]
    public async Task Delete_User_Removes_User()
    {
      var req = new { Name = "DeleteMe", UserId = 987654321L };
      var create = await _client.PostAsJsonAsync("/api/users/login", req);
      create.EnsureSuccessStatusCode();
      var cookie = create.Headers.GetValues("Set-Cookie");
      _client.DefaultRequestHeaders.Remove("Cookie");
      _client.DefaultRequestHeaders.Add("Cookie", string.Join("; ", cookie));
      var user = await create.Content.ReadFromJsonAsync<User>();
      var del = await _client.DeleteAsync($"/api/users/{user.UserId}");
      Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
    }

    [Fact]
    public async Task GetMyTodos_Returns_Empty_For_New_User()
    {
      var req = new { Name = "TodoUser", UserId = 555555555L };
      var login = await _client.PostAsJsonAsync("/api/users/login", req);
      login.EnsureSuccessStatusCode();
      var cookie = login.Headers.GetValues("Set-Cookie");
      _client.DefaultRequestHeaders.Remove("Cookie");
      _client.DefaultRequestHeaders.Add("Cookie", string.Join("; ", cookie));
      var todosResp = await _client.GetAsync("/api/users/me/todos");
      Assert.Equal(HttpStatusCode.OK, todosResp.StatusCode);
      var todos = await todosResp.Content.ReadFromJsonAsync<List<Todo>>();
      Assert.NotNull(todos);
      Assert.Empty(todos);
    }
  }
}
