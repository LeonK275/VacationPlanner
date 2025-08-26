using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Data;
using VacationPlanner.Models;

namespace VacationPlanner.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db) => _db = db;

    // GET: /Employees
    public async Task<IActionResult> Index()
    {
        var employees = await _db.Employees.AsNoTracking().ToListAsync();
        return View(employees);
    }

    // GET: /Employees/Create
    public IActionResult Create() => View(new Employee());

    // POST: /Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.Employees.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
