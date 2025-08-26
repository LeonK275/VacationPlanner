namespace VacationPlanner.Models;

public class VacationRequest
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public string ProjectId { get; set; } = default!;
    public Project? Project { get; set; }

    public DateOnly Start { get; set; }
    public DateOnly End   { get; set; }

    public VacationRequestStatus Status { get; set; }
}