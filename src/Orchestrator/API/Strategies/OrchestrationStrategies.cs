namespace Orchestrator.API.Strategies;

/// <summary>
/// Strategy interface for selecting LLM models based on topic classification.
/// </summary>
public interface IModelSelectionStrategy
{
    /// <summary>
    /// Select the appropriate model based on topic classification.
    /// </summary>
    string SelectModel(string classification);
}

/// <summary>
/// Default model selection strategy that maps topic types to models.
/// </summary>
public class DefaultModelSelectionStrategy : IModelSelectionStrategy
{
    private readonly Dictionary<string, string> _modelMap = new()
    {
        ["Technical"] = "google/gemma-4-26b-a4b-it:free",
        ["General"] = "meta-llama/llama-3.2-3b-instruct:free",
        ["Creative"] = "cognitivecomputations/dolphin3.0-r1-mistral-24b:free"
    };

    public string SelectModel(string classification)
    {
        return _modelMap.GetValueOrDefault(classification, _modelMap["General"]);
    }
}

/// <summary>
/// Strategy interface for classifying topics.
/// </summary>
public interface ITopicClassificationStrategy
{
    /// <summary>
    /// Classify a topic into a category.
    /// </summary>
    string Classify(string topic);
}

/// <summary>
/// Keyword-based topic classification strategy.
/// </summary>
public class KeywordTopicClassificationStrategy : ITopicClassificationStrategy
{
    private readonly Dictionary<string, List<string>> _keywords = new()
    {
        ["Technical"] = new()
        {
            "machine learning", "artificial intelligence", "cloud computing",
            "software", "programming", "api", "docker", "kubernetes",
            "database", "network", "security", "devops", "microservices",
            "algorithm", "data structure", "compiler", "operating system"
        },
        ["Creative"] = new()
        {
            "writing", "storytelling", "poetry", "art", "design",
            "music", "film", "photography", "creative", "brainstorm"
        }
    };

    public string Classify(string topic)
    {
        var lowerTopic = topic.ToLower();

        foreach (var category in _keywords)
        {
            if (category.Value.Any(k => lowerTopic.Contains(k)))
            {
                return category.Key;
            }
        }

        return "General";
    }
}

/// <summary>
/// Strategy interface for orchestrating pipeline execution.
/// </summary>
public interface IOrchestrationStrategy
{
    /// <summary>
    /// Execute the pipeline with the given context.
    /// </summary>
    Task<PipelineResult> ExecuteAsync(PipelineContext context);
}

/// <summary>
/// Sequential orchestration strategy (default).
/// Executes stages in order: Research → Summarize → Report → Notify.
/// </summary>
public class SequentialOrchestrationStrategy : IOrchestrationStrategy
{
    private readonly HttpClient _researchClient;
    private readonly HttpClient _reporterClient;
    private readonly HttpClient _notifierClient;
    private readonly ILogger<SequentialOrchestrationStrategy> _logger;

    public SequentialOrchestrationStrategy(
        IHttpClientFactory httpClientFactory,
        ILogger<SequentialOrchestrationStrategy> logger)
    {
        _researchClient = httpClientFactory.CreateClient("Research");
        _reporterClient = httpClientFactory.CreateClient("Reporter");
        _notifierClient = httpClientFactory.CreateClient("Notifier");
        _logger = logger;
    }

    public async Task<PipelineResult> ExecuteAsync(PipelineContext context)
    {
        try
        {
            // Stage 1-2: Research & Summarize
            context.Status.UpdateStage("Researching", 20);
            var researchResult = await ExecuteResearchAsync(context);
            context.Research = researchResult.Research;
            context.Summary = researchResult.Summary;

            // Stage 3: Report
            context.Status.UpdateStage("Reporting", 50);
            var reportResult = await ExecuteReportAsync(context);
            context.ReportUrl = reportResult.BlobUrl;
            context.BlockchainHash = reportResult.OutputHash;

            // Stage 4-5: Notify & Blockchain
            context.Status.UpdateStage("Notifying", 80);
            var notifyResult = await ExecuteNotifyAsync(context);
            context.TransactionHash = notifyResult.TxHash;

            context.Complete();

            return new PipelineResult
            {
                TaskId = context.TaskId,
                Status = "Completed",
                ReportUrl = context.ReportUrl,
                TxHash = context.TransactionHash,
                Message = "Pipeline completed successfully"
            };
        }
        catch (Exception ex)
        {
            context.Fail(ex.Message);
            throw;
        }
    }

    private async Task<ResearchResult> ExecuteResearchAsync(PipelineContext context)
    {
        var response = await _researchClient.PostAsJsonAsync("/api/research", new
        {
            TaskId = context.TaskId,
            Topic = context.Topic
        });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ResearchResult>();
        return result ?? throw new Exception("Research returned null");
    }

    private async Task<ReportResult> ExecuteReportAsync(PipelineContext context)
    {
        var response = await _reporterClient.PostAsJsonAsync("/api/report", new
        {
            TaskId = context.TaskId,
            Topic = context.Topic,
            Summary = context.Summary
        });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReportResult>();
        return result ?? throw new Exception("Reporter returned null");
    }

    private async Task<NotifyResult> ExecuteNotifyAsync(PipelineContext context)
    {
        var response = await _notifierClient.PostAsJsonAsync("/api/notify", new
        {
            TaskId = context.TaskId,
            Email = context.NotifyEmail,
            ReportUrl = context.ReportUrl,
            OutputHash = context.BlockchainHash
        });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NotifyResult>();
        return result ?? throw new Exception("Notifier returned null");
    }
}

/// <summary>
/// Result types for pipeline stages.
/// </summary>
public class PipelineResult
{
    public string TaskId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ReportUrl { get; set; } = string.Empty;
    public string TxHash { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public record ResearchResult(string Research, string Summary);
public record ReportResult(string BlobUrl, string OutputHash);
public record NotifyResult(string TxHash);
