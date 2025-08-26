using VacationPlanner.Models;

namespace VacationPlanner.Models;

public class EmployeeProject
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public string ProjectId { get; set; } = default!;
    public Project Project { get; set; } = default!;
}