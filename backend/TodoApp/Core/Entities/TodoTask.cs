namespace TodoApp.Core.Entities
{
  public class TodoTask
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }

    public void Toggle()
    {
      IsCompleted = !IsCompleted;
    }
  }
}