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

    public List<Todo> GetAll(long userId) => _repo.GetAll(userId);
    public Todo? GetById(int id, long userId) => _repo.GetById(id, userId);
    public Todo Add(string todoTitle, DateTime? dueDate, long userId) => _repo.Add(todoTitle, dueDate, userId);
    public bool Delete(int id, long userId) => _repo.Delete(id, userId);
    public bool SetDueDate(int id, DateTime? dueDate, long userId) => _repo.SetDueDate(id, dueDate, userId);
  }
}