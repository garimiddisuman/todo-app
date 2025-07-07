using TodoApp.Core.Entities;
using System.Collections.Generic;

namespace TodoApp.Core.Interfaces
{
  public interface IUsersRepository
  {
    User? GetByName(string name);
    User Add(string name, long? userId = null);
    List<User> GetAll();
    bool Delete(long userId);
  }
}
