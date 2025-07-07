using TodoApp.Core.Entities;
using Xunit;

namespace TodoApp.test.Core.Entities
{
  public class TodoTaskTests
  {
    [Fact]
    public void Toggle_ShouldChangeIsCompletedFromFalseToTrue()
    {
      var task = new TodoTask { Id = 1, Title = "Test", IsCompleted = false };
      task.Toggle();
      Assert.True(task.IsCompleted);
    }

    [Fact]
    public void Toggle_ShouldChangeIsCompletedFromTrueToFalse()
    {
      var task = new TodoTask { Id = 2, Title = "Test", IsCompleted = true };
      task.Toggle();
      Assert.False(task.IsCompleted);
    }

    [Fact]
    public void Toggle_ShouldBeIdempotentOverTwoCalls()
    {
      var task = new TodoTask { Id = 3, Title = "Test", IsCompleted = false };
      task.Toggle();
      task.Toggle();
      Assert.False(task.IsCompleted);
    }

    [Fact]
    public void Toggle_ShouldWorkWithRequiredTitle()
    {
      var task = new TodoTask { Id = 4, Title = string.Empty, IsCompleted = false };
      task.Toggle();
      Assert.True(task.IsCompleted);
    }

    [Fact]
    public void Toggle_ShouldNotThrowForLongTitle()
    {
      var longTitle = new string('a', 1000);
      var task = new TodoTask { Id = 5, Title = longTitle, IsCompleted = false };
      var exception = Record.Exception(() => task.Toggle());
      Assert.Null(exception);
    }

    [Fact]
    public void Toggle_ShouldNotThrowForNegativeId()
    {
      var task = new TodoTask { Id = -1, Title = "Test", IsCompleted = false };
      var exception = Record.Exception(() => task.Toggle());
      Assert.Null(exception);
    }
  }
}
