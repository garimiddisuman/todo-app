using Xunit;
using TodoApp.Core.Entities;
using TodoApp.Infrastructure.Repositories;
using System;
using System.Globalization;

namespace TodoApp.test.Core.Entities
{
  public class TodoTests
  {
    [Fact]
    public void AddTask_ShouldAddTaskWithCorrectTitleAndDefaultIsCompleted()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("My Todo");
      todo.AddTask("Task 1");
      Assert.Single(todo.AllTasks);
      Assert.Equal("Task 1", todo.AllTasks[0].Title);
      Assert.False(todo.AllTasks[0].IsCompleted);
    }

    [Fact]
    public void DeleteTask_ShouldRemoveTask_WhenTaskExists()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("My Todo");
      todo.AddTask("Task 1");
      var taskId = todo.AllTasks[0].Id;
      var result = todo.DeleteTask(taskId);
      Assert.True(result);
      Assert.Empty(todo.AllTasks);
    }

    [Fact]
    public void DeleteTask_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("My Todo");
      var result = todo.DeleteTask(999);
      Assert.False(result);
    }

    [Fact]
    public void ToggleTask_ShouldToggleIsCompleted_WhenTaskExists()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("My Todo");
      todo.AddTask("Task 1");
      var taskId = todo.AllTasks[0].Id;
      var result = todo.ToggleTask(taskId);
      Assert.True(result);
      Assert.True(todo.AllTasks[0].IsCompleted);
      todo.ToggleTask(taskId);
      Assert.False(todo.AllTasks[0].IsCompleted);
    }

    [Fact]
    public void ToggleTask_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
      var repo = new TodosRepository();
      var todo = repo.Add("My Todo");
      var result = todo.ToggleTask(999);
      Assert.False(result);
    }

    [Fact]
    public void DueDate_CanBeSet_AndUpdated()
    {
      var todo = new Todo(1, "Test");
      Assert.Null(todo.DueDate);
      var due = DateTime.UtcNow.AddDays(1);
      todo.SetDueDate(due);
      Assert.Equal(due, todo.DueDate);
      var newDue = DateTime.UtcNow.AddDays(2);
      todo.SetDueDate(newDue);
      Assert.Equal(newDue, todo.DueDate);
    }
  }
}
