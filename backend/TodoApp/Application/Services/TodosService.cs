using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;
using System.Collections.Generic;
using System;

namespace TodoApp.Application.Services
{
  public class TodosService
  {
    private readonly ITodosRepository _repo;
    public TodosService(ITodosRepository repo)
    {
      _repo = repo;
    }

    public List<Todo> GetAll() => _repo.GetAll();
    public Todo? GetById(int id) => _repo.GetById(id);
    public Todo Add(string todoTitle, DateTime? dueDate = null) => _repo.Add(todoTitle, dueDate);
    public bool Delete(int id) => _repo.Delete(id);
    public bool SetDueDate(int id, DateTime? dueDate) => _repo.SetDueDate(id, dueDate);
  }
}