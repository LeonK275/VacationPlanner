namespace VacationPlanner.Models;

public class Project
{
    public string Id { get; set; } = default!;    

    // Kunde
    public string CustomerId { get; set; } = default!;
    public Customer? Customer { get; set; }

    // Projektzeitraum
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
}
