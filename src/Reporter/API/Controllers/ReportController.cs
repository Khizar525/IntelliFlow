// ============================================================
// Module 3: Reporter Agent — formats report + Azure Blob + SQL
// Owner: Hassan Asif (02-131232-113)
// ============================================================
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly ReportService _reporter;

    public ReportController(ReportService reporter)
    {
        _reporter = reporter;
    }

    /// <summary>
    /// POST /api/report
    /// Receives summary → formats document → uploads to Blob → persists to SQL
    /// Returns: { "blobUrl": "...", "outputHash": "..." }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] ReportRequest request)
    {
       var result = await _reporter.GenerateReportAsync(request.TaskId, request.Topic, request.Summary);
        return Ok(result);
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "Reporter OK" });
}

public class ReportRequest
{
    public string TaskId  { get; set; } = string.Empty;
    public string Topic   { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}
