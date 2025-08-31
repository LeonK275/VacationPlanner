using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Data;
using VacationPlanner.Models;

namespace VacationPlanner.Controllers;

public class VacationRequestsController : Controller
{
    private readonly AppDbContext _db;
    public VacationRequestsController(AppDbContext db) => _db = db;

    // GET: /VacationRequests
    public async Task<IActionResult> Index()
    {
        var requests = await _db.VacationRequests
            .Include(v => v.Employee)
            .Include(v => v.Project)
            .AsNoTracking()
            .ToListAsync();

        return View(requests);
    }

    // GET: /VacationRequests/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = new SelectList(await _db.Employees.ToListAsync(), "Id", "Name");
        ViewBag.Projects = new SelectList(await _db.Projects.ToListAsync(), "Id", "Id");
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(VacationRequestStatus)));

        return View(new VacationRequest
        {
            Start = DateOnly.FromDateTime(DateTime.Today),
            End = DateOnly.FromDateTime(DateTime.Today)
        });
    }

    // POST: /VacationRequests/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VacationRequest model)
    {
        
        if (model.End < model.Start)
            ModelState.AddModelError(nameof(model.End), "Ende darf nicht vor Start liegen.");

        
        var isAssigned = await _db.EmployeeProjects
            .AnyAsync(ep => ep.EmployeeId == model.EmployeeId && ep.ProjectId == model.ProjectId);

        if (!isAssigned)
            ModelState.AddModelError(nameof(model.ProjectId),
                "Mitarbeiter ist diesem Projekt nicht zugeordnet.");

        
        var conflicts = await FindConflictsForSubmission(model);
        if (conflicts.Count > 0)
        {
            var details = string.Join(", ",
                conflicts.Select(c => $"{c.Employee!.Name} ({c.Start:yyyy-MM-dd}–{c.End:yyyy-MM-dd})"));
            ModelState.AddModelError(string.Empty,
                $"Konflikt mit genehmigten Anträgen im Projekt: {details}");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Employees = new SelectList(_db.Employees, "Id", "Name", model.EmployeeId);
            ViewBag.Projects  = new SelectList(_db.Projects, "Id", "Id", model.ProjectId);
            ViewBag.Statuses  = new SelectList(Enum.GetValues(typeof(VacationRequestStatus)), model.Status);
            return View(model);
        }

        _db.VacationRequests.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    
    private async Task<List<VacationRequest>> FindConflictsForSubmission(VacationRequest candidate)
    {
        
        return await _db.VacationRequests
            .Include(v => v.Employee)
            .Where(v =>
                v.ProjectId == candidate.ProjectId &&
                v.EmployeeId != candidate.EmployeeId &&              // andere Teammitglieder
                v.Status == VacationRequestStatus.Approved &&        // nur genehmigte
                v.Start <= candidate.End && candidate.Start <= v.End // Zeiträume überschneiden
            )
            .AsNoTracking()
            .ToListAsync();
    }
    
    // GET: /VacationRequests/Approve/5
    // GET: /VacationRequests/Approve/5
    public async Task<IActionResult> Approve(int id)
    {
        var request = await _db.VacationRequests
            .Include(v => v.Employee)
            .Include(v => v.Project)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (request == null)
            return NotFound();

        
        var conflicts = await FindConflictsForApproval(request);
        if (conflicts.Count > 0)
        {
            var details = string.Join(", ",
                conflicts.Select(c => $"{c.Employee!.Name} ({c.Start:yyyy-MM-dd}–{c.End:yyyy-MM-dd}, {c.Status})"));

            TempData["Error"] = $"Konflikt mit anderen Anträgen im Projekt: {details}";
            return RedirectToAction(nameof(Index));
        }

        request.Status = VacationRequestStatus.Approved;
        await _db.SaveChangesAsync();

        TempData["Message"] = "Antrag wurde genehmigt.";
        return RedirectToAction(nameof(Index));
    }


    private async Task<List<VacationRequest>> FindConflictsForApproval(VacationRequest candidate)
    {
        return await _db.VacationRequests
            .Include(v => v.Employee)
            .Where(v =>
                    v.ProjectId == candidate.ProjectId &&
                    v.EmployeeId != candidate.EmployeeId &&              // andere Teammitglieder
                    v.Start <= candidate.End && candidate.Start <= v.End // Zeitraum überschneidet sich
            )
            .AsNoTracking()
            .ToListAsync();
    }
    
    // GET: /VacationRequests/Reject/5
    public async Task<IActionResult> Reject(int id)
    {
        var request = await _db.VacationRequests.FindAsync(id);
        if (request == null) return NotFound();
        if (request.Status != VacationRequestStatus.Submitted)
        {
            TempData["Error"] = "Nur eingereichte Anträge können abgelehnt werden.";
            return RedirectToAction(nameof(Index));
        }
        request.Status = VacationRequestStatus.Rejected;
        await _db.SaveChangesAsync();
        TempData["Message"] = "Antrag wurde abgelehnt.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /VacationRequests/Cancel/5
    public async Task<IActionResult> Cancel(int id)
    {
        var request = await _db.VacationRequests.FindAsync(id);
        if (request == null) return NotFound();
        if (request.Status == VacationRequestStatus.Approved ||
            request.Status == VacationRequestStatus.Submitted)
        {
            request.Status = VacationRequestStatus.Cancelled;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Antrag wurde storniert.";
            return RedirectToAction(nameof(Index));
        }
        TempData["Error"] = "Nur eingereichte oder genehmigte Anträge können storniert werden.";
        return RedirectToAction(nameof(Index));
    }



}
