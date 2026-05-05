// ============================================================
// Module 4: Notifier Agent — Email + Blockchain
// Owner: Shamraiz (02-131232-112)
// ============================================================
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NotifyController : ControllerBase
{
    private readonly EmailService      _email;
    private readonly BlockchainService _blockchain;

    public NotifyController(EmailService email, BlockchainService blockchain)
    {
        _email      = email;
        _blockchain = blockchain;
    }

    /// <summary>
    /// POST /api/notify
    /// Sends email to user AND writes hash to Sepolia blockchain.
    /// Returns: { "txHash": "0x..." }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Notify([FromBody] NotifyRequest request)
    {
        // Run email and blockchain in parallel for efficiency
        var emailTask      = _email.SendReportEmailAsync(request.Email, request.ReportUrl, request.TaskId);
        var blockchainTask = _blockchain.LogTaskHashAsync(request.TaskId, request.OutputHash);

        await Task.WhenAll(emailTask, blockchainTask);

        var txHash = await blockchainTask;
        return Ok(new { txHash });
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "Notifier OK" });
}

public class NotifyRequest
{
    public string TaskId     { get; set; } = string.Empty;
    public string Email      { get; set; } = string.Empty;
    public string ReportUrl  { get; set; } = string.Empty;
    public string OutputHash { get; set; } = string.Empty;
}
