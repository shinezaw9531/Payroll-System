namespace PayrollBE.Application.DTOs;

public record PayrollGenerateRequest(DateTime StartDate, DateTime EndDate);

public record PayrollSummaryDto(
    int InstructorId,
    string InstructorName,
    decimal ClassEarnings,
    decimal Commission,
    decimal Bonus,
    decimal Adjustment,
    decimal FinalPayout
);

public record ClassBreakdownDto(int ClassId, string Title, DateTime Date, string Status, decimal Earnings);
public record CommissionBreakdownDto(int SaleId, string Type, decimal Amount, decimal Earned, bool IsRefunded);

public record InstructorPayrollDetailDto(
    int InstructorId,
    string InstructorName,
    decimal TotalClassEarnings,
    decimal TotalCommission,
    decimal TotalBonus,
    decimal TotalAdjustment,
    decimal FinalPayout,
    List<ClassBreakdownDto> Classes,
    List<CommissionBreakdownDto> Commissions
);