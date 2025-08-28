using Microsoft.AspNetCore.Mvc;
using VacationPlanner.Models;

namespace VacationPlanner.Controllers;

public class ImportController(ImportService importService) : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Bitte eine JSON-Datei auswählen.");
            return View();
        }

        using var stream = file.OpenReadStream();
        var result = await importService.ImportAsync(stream, ct);
        return View("Result", result);
    }
}