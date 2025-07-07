using System;
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
  public class TodosControllerTests : IClassFixture<WebApplicationFactory<TodoApp.Program>>
  {
    private readonly HttpClient _client;
    private readonly string _cookieHeader;

    public TodosControllerTests(WebApplicationFactory<TodoApp.Program> factory)
    {
      var loginClient = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new System.Uri("http://localhost:8000") });
      var loginReq = new { Name = "TestUser", UserId = 123456789L };
      var loginResp = loginClient.PostAsJsonAsync("/api/users/login", loginReq).Result;
      loginResp.EnsureSuccessStatusCode();
      _cookieHeader = string.Join("; ", loginResp.Headers.GetValues("Set-Cookie"));
      _client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new System.Uri("http://localhost:8000") });
      _client.DefaultRequestHeaders.Add("Cookie", _cookieHeader);
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndList()
    {
      var response = await _client.GetAsync("/api/todos");
      response.EnsureSuccessStatusCode();
      var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
      Assert.NotNull(todos);
    }

    [Fact]
    public async Task Create_And_Get_Todo()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { TodoTitle = "Test Todo" });
      response.EnsureSuccessStatusCode();
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      Assert.NotNull(created);
      var getResponse = await _client.GetAsync($"/api/todos/{created.TodoId}");
      getResponse.EnsureSuccessStatusCode();
      var fetched = await getResponse.Content.ReadFromJsonAsync<Todo>();
      Assert.Equal(created.TodoId, fetched.TodoId);
    }

    [Fact]
    public async Task Get_NonExistent_Todo_ReturnsNotFound()
    {
      var response = await _client.GetAsync("/api/todos/99999");
      Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Todo_ReturnsNoContent()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { TodoTitle = "ToDelete" });
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var delResponse = await _client.DeleteAsync($"/api/todos/{created.TodoId}");
      Assert.Equal(HttpStatusCode.NoContent, delResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistent_Todo_ReturnsNotFound()
    {
      var delResponse = await _client.DeleteAsync("/api/todos/99999");
      Assert.Equal(HttpStatusCode.NotFound, delResponse.StatusCode);
    }

    [Fact]
    public async Task Add_And_Toggle_Task()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { TodoTitle = "WithTask" });
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var addTaskResp = await _client.PostAsJsonAsync($"/api/todos/{created.TodoId}/tasks", new { Title = "Task 1", DueDate = DateTime.UtcNow.AddDays(1) });
      addTaskResp.EnsureSuccessStatusCode();
      var updated = await addTaskResp.Content.ReadFromJsonAsync<Todo>();
      Assert.Single(updated.AllTasks);
      var taskId = updated.AllTasks[0].Id;
      var toggleResp = await _client.PostAsync($"/api/todos/{created.TodoId}/tasks/{taskId}/toggle", null);
      toggleResp.EnsureSuccessStatusCode();
      var toggled = await toggleResp.Content.ReadFromJsonAsync<Todo>();
      Assert.True(toggled.AllTasks[0].IsCompleted);
      // Toggle back
      var toggleBackResp = await _client.PostAsync($"/api/todos/{created.TodoId}/tasks/{taskId}/toggle", null);
      toggleBackResp.EnsureSuccessStatusCode();
      var toggledBack = await toggleBackResp.Content.ReadFromJsonAsync<Todo>();
      Assert.False(toggledBack.AllTasks[0].IsCompleted);
    }

    [Fact]
    public async Task AddTask_To_NonExistent_Todo_ReturnsNotFound()
    {
      var addTaskResp = await _client.PostAsJsonAsync($"/api/todos/99999/tasks", new { Title = "Task 1", DueDate = DateTime.UtcNow.AddDays(1) });
      Assert.Equal(HttpStatusCode.NotFound, addTaskResp.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_From_NonExistent_Todo_ReturnsNotFound()
    {
      var delTaskResp = await _client.DeleteAsync($"/api/todos/99999/tasks/1");
      Assert.Equal(HttpStatusCode.NotFound, delTaskResp.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistent_Task_ReturnsNotFound()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", "WithTask");
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var delTaskResp = await _client.DeleteAsync($"/api/todos/{created.TodoId}/tasks/99999");
      Assert.Equal(HttpStatusCode.NotFound, delTaskResp.StatusCode);
    }

    [Fact]
    public async Task ToggleTask_NonExistent_Todo_ReturnsNotFound()
    {
      var toggleResp = await _client.PostAsync($"/api/todos/99999/tasks/1/toggle", null);
      Assert.Equal(HttpStatusCode.NotFound, toggleResp.StatusCode);
    }

    [Fact]
    public async Task Toggle_NonExistent_Task_ReturnsNotFound()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", "WithTask");
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var toggleResp = await _client.PostAsync($"/api/todos/{created.TodoId}/tasks/99999/toggle", null);
      Assert.Equal(HttpStatusCode.NotFound, toggleResp.StatusCode);
    }

    [Fact]
    public async Task Create_Todo_Sets_DueDate()
    {
      var dueDate = DateTime.UtcNow.AddDays(1);
      var req = new { TodoTitle = "WithDates", DueDate = dueDate };
      var response = await _client.PostAsJsonAsync("/api/todos", req);
      response.EnsureSuccessStatusCode();
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      Assert.NotNull(created);
      Assert.Equal(dueDate.ToString("o"), created.DueDate?.ToString("o"));
    }

    [Fact]
    public async Task Set_DueDate_Updates_DueDate()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { TodoTitle = "SetDue" });
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var newDue = DateTime.UtcNow.AddDays(2);
      var patchResp = await _client.PatchAsJsonAsync($"/api/todos/{created.TodoId}/duedate", new { DueDate = newDue });
      patchResp.EnsureSuccessStatusCode();
      var updated = await patchResp.Content.ReadFromJsonAsync<Todo>();
      Assert.Equal(newDue.ToString("o"), updated.DueDate?.ToString("o"));
    }

    [Fact]
    public async Task Create_Todo_Without_Title_Should_Fail()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { });
      Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Add_Task_Without_Title_Should_Fail()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { TodoTitle = "TestTodo" });
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var addTaskResp = await _client.PostAsJsonAsync($"/api/todos/{created.TodoId}/tasks", new { });
      Assert.Equal(HttpStatusCode.BadRequest, addTaskResp.StatusCode);
    }

    [Fact]
    public async Task Set_And_Clear_Todo_DueDate()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { TodoTitle = "DueDateTest" });
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var newDue = DateTime.UtcNow.AddDays(2);
      var patchResp = await _client.PatchAsJsonAsync($"/api/todos/{created.TodoId}/duedate", new { DueDate = newDue });
      patchResp.EnsureSuccessStatusCode();
      var updated = await patchResp.Content.ReadFromJsonAsync<Todo>();
      Assert.Equal(newDue.ToString("o"), updated.DueDate?.ToString("o"));
      // Clear due date
      var clearResp = await _client.PatchAsJsonAsync($"/api/todos/{created.TodoId}/duedate", new { DueDate = (DateTime?)null });
      clearResp.EnsureSuccessStatusCode();
      var cleared = await clearResp.Content.ReadFromJsonAsync<Todo>();
      Assert.Null(cleared.DueDate);
    }

    [Fact]
    public async Task Delete_Task_Then_Toggle_Should_Return_NotFound()
    {
      var response = await _client.PostAsJsonAsync("/api/todos", new { TodoTitle = "DeleteToggle" });
      var created = await response.Content.ReadFromJsonAsync<Todo>();
      var addTaskResp = await _client.PostAsJsonAsync($"/api/todos/{created.TodoId}/tasks", new { Title = "Task to delete", DueDate = DateTime.UtcNow.AddDays(1) });
      var updated = await addTaskResp.Content.ReadFromJsonAsync<Todo>();
      var taskId = updated.AllTasks[0].Id;
      var delTaskResp = await _client.DeleteAsync($"/api/todos/{created.TodoId}/tasks/{taskId}");
      Assert.Equal(HttpStatusCode.NoContent, delTaskResp.StatusCode);
      var toggleResp = await _client.PostAsync($"/api/todos/{created.TodoId}/tasks/{taskId}/toggle", null);
      Assert.Equal(HttpStatusCode.NotFound, toggleResp.StatusCode);
    }
  }
}
