namespace PayrollBE.Infrastructure.Persistence;

using PayrollBE.Domain.Entities;
using PayrollBE.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void SeedData(PayrollDbContext context)
    {
        if (context.Instructors.Any()) return;

        var instructors = new List<Instructor> {
            new() { Name = "John Doe", Email = "john@rezerv.com" },
            new() { Name = "Jane Smith", Email = "jane@rezerv.com" },
            new() { Name = "Ace Anderson", Email = "ace@rezerv.com" }
        };
        context.Instructors.AddRange(instructors);
        context.SaveChanges();

        // Add mix of Classes, Bookings, and Sales matching requirements
        var classes = new List<Class> {
            new() { InstructorId = 1, ClassName = "Morning Yoga", ClassType = "Yoga", FixedFee = 30, PayoutPerBooking = 10, Status = "Completed", StartTime = new DateTime(2026, 6, 5) },
            new() { InstructorId = 1, ClassName = "Power HIIT", ClassType = "HIIT", FixedFee = 50, PayoutPerBooking = 10, Status = "Completed", StartTime = new DateTime(2026, 6, 12) },
            new() { InstructorId = 2, ClassName = "Zumba Dance", ClassType = "Zumba", FixedFee = 40, PayoutPerBooking = 5, Status = "Cancelled", StartTime = new DateTime(2026, 6, 14) }
        };
        context.Classes.AddRange(classes);
        context.SaveChanges();

        // Seed 16 Bookings for Class 2 to trigger the Attendance Bonus (> 15)
        for (int i = 1; i <= 17; i++)
        {
            context.Bookings.Add(new Booking { ClassId = 2, ParticipantName = $"Client {i}", Status = i == 17 ? "NoShow" : "Completed" });
        }

        // Regular bookings for Class 1
        context.Bookings.Add(new Booking { ClassId = 1, ParticipantName = "Client A", Status = "Completed" });
        context.Bookings.Add(new Booking { ClassId = 1, ParticipantName = "Client B", Status = "NoShow" });
        context.Bookings.Add(new Booking { ClassId = 1, ParticipantName = "Client C", Status = "Cancelled" });

        // Sales (Commissions and Refund Adjustments)
        context.Sales.AddRange(new List<Sale> {
            new() { InstructorId = 1, ItemType = "Membership", Amount = 200, IsRefunded = false, SaleDate = new DateTime(2026, 6, 4) },
            new() { InstructorId = 1, ItemType = "Package", Amount = 100, IsRefunded = true, SaleDate = new DateTime(2026, 6, 10) }, // Reverses commission
            new() { InstructorId = 2, ItemType = "Membership", Amount = 300, IsRefunded = false, SaleDate = new DateTime(2026, 6, 11) }
        });

        context.SaveChanges();
    }
}