using Xunit;
using TodoApp.Core.Entities;
using TodoApp.Infrastructure.Repositories;

namespace TodoApp.test.Core.Entities
{
  public class TodosTests
  {
    [Fact]
    public void AddTodo_ShouldAddTodoWithIncrementedIdAndTitle()
    {
      var repo = new TodosRepository();
      var todo1 = repo.Add("First", null, 1L);
      var todo2 = repo.Add("Second", null, 1L);
      Assert.Equal(1, todo1.TodoId);
      Assert.Equal("First", todo1.TodoTitle);
      Assert.Equal(2, todo2.TodoId);
      Assert.Equal("Second", todo2.TodoTitle);
      Assert.Equal(2, repo.GetAll(1L).Count);
    }

    [Fact]
    public void DeleteTodo_ShouldRemoveTodo_WhenTodoExists()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("Test", null, 1L);
      var result = repo.Delete(todo.TodoId, 1L);
      Assert.True(result);
      Assert.Empty(repo.GetAll(1L));
    }

    [Fact]
    public void DeleteTodo_ShouldReturnFalse_WhenTodoDoesNotExist()
    {
      var repo = new TodosRepository();
      var result = repo.Delete(999, 1L);
      Assert.False(result);
    }

    [Fact]
    public void GetTodo_ShouldReturnTodo_WhenExists()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("Test", null, 1L);
      var found = repo.GetById(todo.TodoId, 1L);
      Assert.NotNull(found);
      Assert.Equal(todo.TodoId, found.TodoId);
    }

    [Fact]
    public void GetTodo_ShouldReturnNull_WhenNotExists()
    {
      var repo = new TodosRepository();
      var found = repo.GetById(999, 1L);
      Assert.Null(found);
    }

    [Fact]
    public void GetTodo_ShouldReturnNull_WhenListIsEmpty()
    {
      var repo = new TodosRepository();
      var found = repo.GetById(1, 1L);
      Assert.Null(found);
    }

    [Fact]
    public void AddTodo_ShouldAddTodoWithUserId()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("UserTodo", null, 42L);
      Assert.Equal(42L, todo.UserId);
    }

    [Fact]
    public void GetAll_ShouldReturnTodosForSpecificUser()
    {
      var repo = new TodosRepository();
      repo.Add("User1Todo1", null, 1L);
      repo.Add("User1Todo2", null, 1L);
      repo.Add("User2Todo1", null, 2L);
      var user1Todos = repo.GetAll(1L);
      var user2Todos = repo.GetAll(2L);
      Assert.Equal(2, user1Todos.Count);
      Assert.Single(user2Todos);
      Assert.All(user1Todos, t => Assert.Equal(1L, t.UserId));
      Assert.All(user2Todos, t => Assert.Equal(2L, t.UserId));
    }

    [Fact]
    public void Add_ShouldRequireUserId()
    {
      var repo = new TodosRepository();
      Assert.Throws<System.ArgumentException>(() => repo.Add("NoUserId", null, 0L));
    }
  }
}
