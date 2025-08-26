using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Data;
using VacationPlanner.Models;

namespace VacationPlanner.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _db;
        public ProjectsController(AppDbContext db) => _db = db;

        // GET /Projects
        public async Task<IActionResult> Index()
        {
            var items = await _db.Projects
                .Include(p => p.Customer)
                .AsNoTracking()
                .ToListAsync();

            return View(items);
        }

        // GET /Projects/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = new SelectList(
                await _db.Customers.AsNoTracking().ToListAsync(),
                nameof(Customer.Id),
                nameof(Customer.Name)
            );
            return View(new Project());
        }

        // POST /Projects/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project model)
        {
            // Validierungen
            var exists = await _db.Customers.AnyAsync(c => c.Id == model.CustomerId);
            if (!exists)
                ModelState.AddModelError(nameof(Project.CustomerId), "Bitte gültigen Kunden wählen.");
            if (model.End < model.Start)
                ModelState.AddModelError(nameof(Project.End), "Ende darf nicht vor Start liegen.");

            if (!ModelState.IsValid)
            {
                ViewBag.Customers = new SelectList(
                    await _db.Customers.AsNoTracking().ToListAsync(),
                    nameof(Customer.Id), nameof(Customer.Name), model.CustomerId);
                return View(model);
            }

            _db.Projects.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
