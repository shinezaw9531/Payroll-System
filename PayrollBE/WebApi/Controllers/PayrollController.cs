namespace PayrollBE.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using PayrollBE.Application.Services;

public class PayrollRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] PayrollRequest request)
    {
        if (request.StartDate > request.EndDate) return BadRequest("Invalid date window.");
        await _payrollService.GeneratePayrollAsync(request.StartDate, request.EndDate);
        return Ok(new { message = "Payroll compiled successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _payrollService.GetSummaryAsync(startDate, endDate);
        return Ok(result);
    }

    [HttpGet("{instructorId}")]
    public async Task<IActionResult> GetDetail(int instructorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _payrollService.GetInstructorDetailAsync(instructorId, startDate, endDate);
        return Ok(result);
    }
}