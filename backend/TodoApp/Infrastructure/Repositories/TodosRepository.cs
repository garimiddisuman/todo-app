using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;
using System.Collections.Generic;
using System;

namespace TodoApp.Infrastructure.Repositories
{
  public class TodosRepository : ITodosRepository
  {
    private int _nextTodoId = 1;
    private readonly List<Todo> _items = new();

    public List<Todo> GetAll(long userId) => _items.FindAll(t => t.UserId == userId);

    public Todo? GetById(int id, long userId) => _items.Find(t => t.TodoId == id && t.UserId == userId);

    public Todo Add(string todoTitle, DateTime? dueDate, long userId)
    {
      if (userId <= 0)
        throw new ArgumentException("userId must be a positive, non-zero value.", nameof(userId));
      var todo = new Todo(_nextTodoId++, todoTitle, dueDate, userId);
      _items.Add(todo);
      return todo;
    }

    public bool Delete(int id, long userId)
    {
      var todo = GetById(id, userId);
      if (todo == null) return false;
      _items.Remove(todo);
      return true;
    }

    public bool SetDueDate(int id, DateTime? dueDate, long userId)
    {
      var todo = GetById(id, userId);
      if (todo == null) return false;
      todo.SetDueDate(dueDate);
      return true;
    }

    // Old interface methods for compatibility (throw NotImplementedException)
    public bool Delete(int id) => throw new NotImplementedException();
    public bool SetDueDate(int id, DateTime? dueDate) => throw new NotImplementedException();
  }
}