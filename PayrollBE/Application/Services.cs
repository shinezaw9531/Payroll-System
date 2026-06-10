using Microsoft.EntityFrameworkCore;
using PayrollBE.Domain.Services;
using PayrollBE.Infrastructure.Persistence;

namespace PayrollBE.Application.Services;

public interface IPayrollService
{
    Task GeneratePayrollAsync(DateTime start, DateTime end);
    Task<List<PayrollSummaryDto>> GetSummaryAsync(DateTime start, DateTime end);
    Task<object> GetInstructorDetailAsync(int instructorId, DateTime start, DateTime end);
}

public class PayrollSummaryDto
{
    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public decimal ClassEarnings { get; set; }
    public decimal Commission { get; set; }
    public decimal Bonus { get; set; }
    public decimal Adjustment { get; set; }
    public decimal FinalPayout { get; set; }
}

public class PayrollService : IPayrollService
{
    private readonly PayrollDbContext _context;

    public PayrollService(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task GeneratePayrollAsync(DateTime start, DateTime end)
    {
        // Safe Idempotency: Clear down existing calculations for date range to avoid duplicate entries
        var existing = await _context.PayrollSummaries
            .Where(p => p.StartDate == start && p.EndDate == end)
            .ToListAsync();
        if (existing.Any())
        {
            _context.PayrollSummaries.RemoveRange(existing);
            await _context.SaveChangesAsync();
        }

        var policy = (await _context.StudioConfigurations.FirstOrDefaultAsync(c => c.KeyName == "NoShowPayoutPolicy"))?.Value ?? "PayInstructor";
        var instructors = await _context.Instructors.ToListAsync();

        foreach (var instructor in instructors)
        {
            var classes = await _context.Classes
                .Include(c => c.Bookings)
                .Where(c => c.InstructorId == instructor.Id && c.StartTime >= start && c.StartTime <= end)
                .ToListAsync();

            var sales = await _context.Sales
                .Where(s => s.InstructorId == instructor.Id && s.SaleDate >= start && s.SaleDate <= end)
                .ToListAsync();

            decimal classEarnings = classes.Sum(c => PayrollEngine.CalculateClassEarnings(c, policy));
            decimal bonus = classes.Sum(c => PayrollEngine.CalculateAttendanceBonus(c));
            decimal commission = sales.Where(s => !s.IsRefunded).Sum(s => PayrollEngine.CalculateCommission(s));
            decimal adjustments = sales.Where(s => s.IsRefunded).Sum(s => PayrollEngine.CalculateHistoricalAdjustment(s));

            decimal finalPayout = classEarnings + bonus + commission + adjustments;

            _context.PayrollSummaries.Add(new PayrollSummary
            {
                InstructorId = instructor.Id,
                StartDate = start,
                EndDate = end,
                ClassEarnings = classEarnings,
                Bonus = bonus,
                Commission = commission,
                Adjustment = adjustments,
                FinalPayout = finalPayout
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<PayrollSummaryDto>> GetSummaryAsync(DateTime start, DateTime end)
    {
        return await _context.PayrollSummaries
            .Include(p => p.Instructor)
            .Where(p => p.StartDate == start && p.EndDate == end)
            .Select(p => new PayrollSummaryDto
            {
                InstructorId = p.InstructorId,
                InstructorName = p.Instructor!.Name,
                ClassEarnings = p.ClassEarnings,
                Commission = p.Commission,
                Bonus = p.Bonus,
                Adjustment = p.Adjustment,
                FinalPayout = p.FinalPayout
            }).ToListAsync();
    }

    public async Task<object> GetInstructorDetailAsync(int instructorId, DateTime start, DateTime end)
    {
        var instructor = await _context.Instructors.FindAsync(instructorId);
        if (instructor == null) return new { error = "Instructor not found" };

        var classes = await _context.Classes.Include(c => c.Bookings)
            .Where(c => c.InstructorId == instructorId && c.StartTime >= start && c.StartTime <= end).ToListAsync();

        var sales = await _context.Sales
            .Where(s => s.InstructorId == instructorId && s.SaleDate >= start && s.SaleDate <= end).ToListAsync();

        var policy = (await _context.StudioConfigurations.FirstOrDefaultAsync(c => c.KeyName == "NoShowPayoutPolicy"))?.Value ?? "PayInstructor";

        return new
        {
            InstructorName = instructor.Name,
            ClassesTaught = classes.Select(c => new {
                c.ClassName,
                c.Status,
                BookingsCount = c.Bookings.Count,
                CalculatedPayout = PayrollEngine.CalculateClassEarnings(c, policy)
            }),
            CommissionRecords = sales.Where(s => !s.IsRefunded).Select(s => new { s.ItemType, s.Amount, Earned = PayrollEngine.CalculateCommission(s) }),
            RefundAdjustments = sales.Where(s => s.IsRefunded).Select(s => new { s.ItemType, s.Amount, Penalty = PayrollEngine.CalculateHistoricalAdjustment(s) }),
            BonusRecords = classes.Select(c => new { c.ClassName, Bonus = PayrollEngine.CalculateAttendanceBonus(c) }).Where(b => b.Bonus > 0)
        };
    }
}