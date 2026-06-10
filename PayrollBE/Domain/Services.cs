namespace PayrollBE.Domain.Services;

using PayrollBE.Domain.Entities;

public static class PayrollEngine
{
    public static decimal CalculateClassEarnings(Class @class, string noShowPolicy)
    {
        if (@class.Status == "Cancelled") return 0m;

        decimal total = @class.FixedFee;

        foreach (var booking in @class.Bookings)
        {
            if (booking.Status == "Completed")
            {
                total += @class.PayoutPerBooking;
            }
            else if (booking.Status == "NoShow" && noShowPolicy == "PayInstructor")
            {
                total += @class.PayoutPerBooking;
            }
        }
        return total;
    }

    public static decimal CalculateCommission(Sale sale)
    {
        if (sale.IsRefunded) return 0m;

        return sale.ItemType.ToLower() switch
        {
            "membership" => sale.Amount * 0.10m,
            "package" => sale.Amount * 0.05m,
            _ => 0m
        };
    }

    public static decimal CalculateHistoricalAdjustment(Sale sale)
    {
        // If a past sale was marked as refunded, it triggers a negative deduction adjustment
        if (sale.IsRefunded)
        {
            return sale.ItemType.ToLower() switch
            {
                "membership" => -(sale.Amount * 0.10m),
                "package" => -(sale.Amount * 0.05m),
                _ => 0m
            };
        }
        return 0m;
    }

    public static decimal CalculateAttendanceBonus(Class @class)
    {
        if (@class.Status == "Cancelled") return 0m;

        // Attendance count excludes cancelled bookings
        int activeAttendance = @class.Bookings.Count(b => b.Status == "Completed" || b.Status == "NoShow");

        return activeAttendance > 15 ? 20.00m : 0m;
    }
}