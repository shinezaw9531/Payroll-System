

namespace PayrollBE.Domain.Entities;

public class Instructor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Class
{
    public int Id { get; set; }
    public int InstructorId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string ClassType { get; set; } = string.Empty;
    public decimal FixedFee { get; set; }
    public decimal PayoutPerBooking { get; set; }
    public string Status { get; set; } = "Completed"; // Completed, Cancelled
    public DateTime StartTime { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public class Booking
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed"; // Completed, Cancelled, NoShow
}

public class Sale
{
    public int Id { get; set; }
    public int InstructorId { get; set; }
    public string ItemType { get; set; } = string.Empty; // Membership, Package
    public decimal Amount { get; set; }
    public bool IsRefunded { get; set; }
    public DateTime SaleDate { get; set; }
}