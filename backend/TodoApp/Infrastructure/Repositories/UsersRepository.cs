using System.Collections.Generic;
using System.Linq;
using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;

namespace TodoApp.Infrastructure.Repositories
{
  public class UsersRepository : IUsersRepository
  {
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public User? GetByName(string name) => _users.FirstOrDefault(u => u.Name == name);

    public User Add(string name, long? userId = null)
    {
      var id = userId ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      var user = new User(id, name);
      _users.Add(user);
      return user;
    }

    public List<User> GetAll() => _users;

    public bool Delete(long userId)
    {
      var user = _users.FirstOrDefault(u => u.UserId == userId);
      if (user == null) return false;
      _users.Remove(user);
      return true;
    }
  }
}
