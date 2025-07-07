using System.Collections.Generic;
using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;

namespace TodoApp.Application.Services
{
  public class UsersService
  {
    private readonly IUsersRepository _repo;
    public UsersService(IUsersRepository repo)
    {
      _repo = repo;
    }

    public User? GetByName(string name) => _repo.GetByName(name);
    public User Add(string name, long? userId = null) => _repo.Add(name, userId);
    public List<User> GetAll() => _repo.GetAll();
    public bool Delete(long userId) => _repo.Delete(userId);
  }
}
