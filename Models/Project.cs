namespace VacationPlanner.Models;

public class Project
{
    public string Id { get; set; } = default!;    

    
    public string CustomerId { get; set; } = default!;
    public Customer? Customer { get; set; }

    
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public ICollection<EmployeeProject> EmployeeProjects { get; } = new List<EmployeeProject>();
    public ICollection<VacationRequest> VacationRequests { get; } = new List<VacationRequest>();

}
