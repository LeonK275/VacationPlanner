namespace VacationPlanner.Models;

public class Customer
{
    public string Id { get; set; } = default!;   
    public string Name { get; set; } = default!;
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
