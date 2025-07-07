using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp.Core.Entities
{
  public class Todo
  {
    public int TodoId { get; set; }
    public int NextTaskId { get; set; }
    public string TodoTitle { get; set; }
    public List<TodoTask> AllTasks { get; set; } = new List<TodoTask>();
    public DateTime? DueDate { get; set; }
    public long UserId { get; set; }

    public Todo(int todoId, string todoTitle, DateTime? dueDate = null, long userId = 0)
    {
      TodoId = todoId;
      TodoTitle = todoTitle;
      NextTaskId = 1;
      DueDate = dueDate;
      UserId = userId;
    }

    public void AddTask(string title, DateTime? dueDate = null)
    {
      AllTasks.Add(new TodoTask
      {
        Id = NextTaskId++,
        Title = title,
        IsCompleted = false,
        DueDate = dueDate
      });
    }

    public bool DeleteTask(int taskId)
    {
      var task = AllTasks.FirstOrDefault(t => t.Id == taskId);
      if (task == null) return false;
      AllTasks.Remove(task);
      return true;
    }

    public bool ToggleTask(int taskId)
    {
      var task = AllTasks.FirstOrDefault(t => t.Id == taskId);
      if (task == null) return false;
      task.Toggle();
      return true;
    }

    public void SetDueDate(DateTime? dueDate)
    {
      DueDate = dueDate;
    }
  }
}