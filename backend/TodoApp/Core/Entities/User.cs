using System.Collections.Generic;

namespace TodoApp.Core.Entities
{
  public class User
  {
    public long UserId { get; set; }
    public string Name { get; set; }
    public List<Todo> Todos { get; set; } = new List<Todo>();

    public User(long userId, string name)
    {
      UserId = userId;
      Name = name;
    }
  }
}
