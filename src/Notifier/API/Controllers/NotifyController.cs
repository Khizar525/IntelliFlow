using Microsoft.AspNetCore.Mvc;
using Notifier.API.Services;

namespace Notifier.API.Controllers;

[ApiController]
[Route("api/notify")]
public class NotifyController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly BlockchainService _blockchainService;

    public NotifyController(EmailService emailService, BlockchainService blockchainService)
    {
        _emailService = emailService;
        _blockchainService = blockchainService;
    }

    [HttpPost]
    public async Task<IActionResult> Notify([FromBody] NotifyRequest request)
    {
        // 1. Send email
        await _emailService.SendReportEmailAsync(
            request.Email, 
            request.TaskId, 
            request.ReportUrl
        );

        // 2. Log to blockchain
        var txHash = await _blockchainService.LogTaskHashAsync(
            request.TaskId, 
            request.OutputHash
        );

        return Ok(new { txHash });
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok("Notifier is running");
}

public class NotifyRequest
{
    public string TaskId { get; set; } = "";
    public string Email { get; set; } = "";
    public string ReportUrl { get; set; } = "";
    public string OutputHash { get; set; } = "";
}