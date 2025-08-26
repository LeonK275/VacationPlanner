using Microsoft.EntityFrameworkCore;
using VacationPlanner.Models;

namespace VacationPlanner.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<EmployeeProject> EmployeeProjects => Set<EmployeeProject>();
    public DbSet<VacationRequest> VacationRequests => Set<VacationRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // EmployeeProject Mapping
        modelBuilder.Entity<EmployeeProject>()
            .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

        modelBuilder.Entity<EmployeeProject>()
            .HasOne(ep => ep.Employee)
            .WithMany(e => e.EmployeeProjects)
            .HasForeignKey(ep => ep.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeProject>()
            .HasOne(ep => ep.Project)
            .WithMany(p => p.EmployeeProjects)
            .HasForeignKey(ep => ep.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // VacationRequest Mapping
        modelBuilder.Entity<VacationRequest>(entity =>
        {
            entity.HasOne(v => v.Employee)
                  .WithMany(e => e.VacationRequests)
                  .HasForeignKey(v => v.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Project)
                  .WithMany(p => p.VacationRequests)
                  .HasForeignKey(v => v.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Status als TEXT speichern
            entity.Property(v => v.Status)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            // Indizes für Konfliktprüfung
            entity.HasIndex(v => new { v.ProjectId, v.Start, v.End });
            entity.HasIndex(v => new { v.EmployeeId, v.Start, v.End });

            // Constraint: Start <= End
            entity.HasCheckConstraint("CK_VacationRequest_Period", "\"Start\" <= \"End\"");
        });
    }
}
