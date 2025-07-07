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

    public List<Todo> GetAll() => _items;

    public Todo? GetById(int id) => _items.Find(t => t.TodoId == id);

    public Todo Add(string todoTitle, DateTime? dueDate = null)
    {
      var todo = new Todo(_nextTodoId++, todoTitle, dueDate);
      _items.Add(todo);
      return todo;
    }

    public bool Delete(int id)
    {
      var todo = GetById(id);
      if (todo == null) return false;
      _items.Remove(todo);
      return true;
    }

    public bool SetDueDate(int id, DateTime? dueDate)
    {
      var todo = GetById(id);
      if (todo == null) return false;
      todo.SetDueDate(dueDate);
      return true;
    }
  }
}