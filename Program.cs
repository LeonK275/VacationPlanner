using Microsoft.EntityFrameworkCore;
using VacationPlanner.Data;
using VacationPlanner.Models; 

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<VacationPlanner.Models.ImportService>();
        builder.Services.AddScoped<ImportService>(); 

        var connectionString = "Data Source=vacationplanner.db";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}