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
      var todo1 = repo.Add("First");
      var todo2 = repo.Add("Second");
      Assert.Equal(1, todo1.TodoId);
      Assert.Equal("First", todo1.TodoTitle);
      Assert.Equal(2, todo2.TodoId);
      Assert.Equal("Second", todo2.TodoTitle);
      Assert.Equal(2, repo.GetAll().Count);
    }

    [Fact]
    public void DeleteTodo_ShouldRemoveTodo_WhenTodoExists()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("Test");
      var result = repo.Delete(todo.TodoId);
      Assert.True(result);
      Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void DeleteTodo_ShouldReturnFalse_WhenTodoDoesNotExist()
    {
      var repo = new TodosRepository();
      var result = repo.Delete(999);
      Assert.False(result);
    }

    [Fact]
    public void GetTodo_ShouldReturnTodo_WhenExists()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("Test");
      var found = repo.GetById(todo.TodoId);
      Assert.NotNull(found);
      Assert.Equal(todo.TodoId, found.TodoId);
    }

    [Fact]
    public void GetTodo_ShouldReturnNull_WhenNotExists()
    {
      var repo = new TodosRepository();
      var found = repo.GetById(999);
      Assert.Null(found);
    }

    [Fact]
    public void GetTodo_ShouldReturnNull_WhenListIsEmpty()
    {
      var repo = new TodosRepository();
      var found = repo.GetById(1);
      Assert.Null(found);
    }
  }
}
