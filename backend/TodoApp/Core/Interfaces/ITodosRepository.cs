using TodoApp.Core.Entities;
using System.Collections.Generic;
using System;

namespace TodoApp.Core.Interfaces
{
  public interface ITodosRepository
  {
    List<Todo> GetAll(long userId);
    Todo? GetById(int id, long userId);
    Todo Add(string todoTitle, DateTime? dueDate, long userId);
    bool Delete(int id, long userId);
    bool SetDueDate(int id, DateTime? dueDate, long userId);
  }
}