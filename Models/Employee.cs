namespace VacationPlanner.Models;

public class Employee
{
    public int Id { get; set; }           // PK
    public string Name { get; set; } = "";
    public string JobTitle { get; set; } = "";
    
    public ICollection<EmployeeProject> EmployeeProjects { get; } = new List<EmployeeProject>();
    public ICollection<VacationRequest> VacationRequests { get; } = new List<VacationRequest>();

}
