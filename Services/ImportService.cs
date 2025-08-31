using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Data;

namespace VacationPlanner.Models; 
public class ImportResult
{
    public int EmployeesInserted { get; set; }
    public int CustomersInserted { get; set; }
    public int ProjectsInserted { get; set; }
    public int AssignmentsInserted { get; set; }
    public List<string> Errors { get; } = new();
    public bool Success => Errors.Count == 0;
}

public class ImportService
{
    private readonly AppDbContext _db;
    public ImportService(AppDbContext db) => _db = db;

    public async Task<ImportResult> ImportAsync(Stream jsonStream, CancellationToken ct = default)
    {
        var result = new ImportResult();
        ImportPayload? payload;

        try
        {
            payload = await JsonSerializer.DeserializeAsync<ImportPayload>(
                jsonStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                ct);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"JSON konnte nicht gelesen werden: {ex.Message}");
            return result;
        }

        if (payload is null)
        {
            result.Errors.Add("Leerer Import.");
            return result;
        }

        using var tx = await _db.Database.BeginTransactionAsync(ct);

        // Employees (Insert-only)
        foreach (var e in payload.Employees)
        {
            if (string.IsNullOrWhiteSpace(e.Id)) { result.Errors.Add("Employee ohne id."); continue; }
            if (await _db.Employees.AnyAsync(x => x.ExternalId == e.Id, ct))
            {
                result.Errors.Add($"Employee '{e.Id}' existiert bereits (ExternalId).");
                continue;
            }
            _db.Employees.Add(new Employee { ExternalId = e.Id, Name = e.Name, JobTitle = e.JobTitle });
            result.EmployeesInserted++;
        }

        // Customers
        foreach (var c in payload.Customers)
        {
            if (string.IsNullOrWhiteSpace(c.Id)) { result.Errors.Add("Customer ohne id."); continue; }
            if (await _db.Customers.AnyAsync(x => x.Id == c.Id, ct))
            {
                result.Errors.Add($"Customer '{c.Id}' existiert bereits.");
                continue;
            }
            _db.Customers.Add(new Customer { Id = c.Id, Name = c.Name });
            result.CustomersInserted++;
        }
        await _db.SaveChangesAsync(ct);

        // Projects
        foreach (var p in payload.Projects)
        {
            if (string.IsNullOrWhiteSpace(p.Id)) { result.Errors.Add("Project ohne id."); continue; }
            if (await _db.Projects.AnyAsync(x => x.Id == p.Id, ct))
            { result.Errors.Add($"Project '{p.Id}' existiert bereits."); continue; }

            if (!await _db.Customers.AnyAsync(x => x.Id == p.CustomerId, ct))
            { result.Errors.Add($"Project '{p.Id}': Customer '{p.CustomerId}' existiert nicht."); continue; }

            if (p.Period.End < p.Period.Start)
            { result.Errors.Add($"Project '{p.Id}': period.end < period.start."); continue; }

            _db.Projects.Add(new Project { Id = p.Id, CustomerId = p.CustomerId, Start = p.Period.Start, End = p.Period.End });
            result.ProjectsInserted++;
        }
        await _db.SaveChangesAsync(ct);

        // Assignments
        foreach (var p in payload.Projects)
        {
            var project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == p.Id, ct);
            if (project is null) continue;

            foreach (var empExt in p.AssignedEmployeeIds.Distinct())
            {
                var emp = await _db.Employees.FirstOrDefaultAsync(x => x.ExternalId == empExt, ct);
                if (emp is null) { result.Errors.Add($"Project '{p.Id}': Employee '{empExt}' nicht gefunden."); continue; }

                var exists = await _db.EmployeeProjects.AnyAsync(x => x.EmployeeId == emp.Id && x.ProjectId == project.Id, ct);
                if (exists) { result.Errors.Add($"Project '{p.Id}': Employee '{empExt}' bereits zugeordnet."); continue; }

                _db.EmployeeProjects.Add(new EmployeeProject { EmployeeId = emp.Id, ProjectId = project.Id });
                result.AssignmentsInserted++;
            }
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result;
    }
}
