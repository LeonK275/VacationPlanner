using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Data;
using VacationPlanner.Models;

namespace VacationPlanner.Controllers;

public class CustomersController(AppDbContext db) : Controller
{
    // GET /Customers
    public async Task<IActionResult> Index() =>
        View(await db.Customers.AsNoTracking().ToListAsync());

    // GET /Customers/Create
    public IActionResult Create() => View(new Customer());

    // POST /Customers/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model, [FromServices] AppDbContext db)
    {
        if (!ModelState.IsValid) return View(model);
        db.Customers.Add(model);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
