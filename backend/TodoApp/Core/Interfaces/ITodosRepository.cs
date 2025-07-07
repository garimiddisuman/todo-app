using TodoApp.Core.Entities;
using System.Collections.Generic;
using System;

namespace TodoApp.Core.Interfaces
{
  public interface ITodosRepository
  {
    List<Todo> GetAll();
    Todo? GetById(int id);
    Todo Add(string todoTitle, DateTime? dueDate = null);
    bool Delete(int id);
    bool SetDueDate(int id, DateTime? dueDate);
  }
}