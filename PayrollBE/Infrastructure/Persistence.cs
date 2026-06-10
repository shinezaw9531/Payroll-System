namespace PayrollBE.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using PayrollBE.Domain.Entities;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options) { }

    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<StudioConfigurations> StudioConfigurations => Set<StudioConfigurations>();
    public DbSet<PayrollSummary> PayrollSummaries => Set<PayrollSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PayrollSummary>()
            .HasIndex(p => new { p.InstructorId, p.StartDate, p.EndDate })
            .IsUnique();
    }
}

public class StudioConfigurations
{
    [System.ComponentModel.DataAnnotations.Key]
    public string KeyName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class PayrollSummary
{
    public int Id { get; set; }
    public int InstructorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal ClassEarnings { get; set; }
    public decimal Commission { get; set; }
    public decimal Bonus { get; set; }
    public decimal Adjustment { get; set; }
    public decimal FinalPayout { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public Instructor? Instructor { get; set; }
}