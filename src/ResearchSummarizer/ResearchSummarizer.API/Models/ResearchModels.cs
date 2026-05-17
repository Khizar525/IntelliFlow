namespace ResearchSummarizer.API.Models;

public class ResearchRequest
{
    public string TaskId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}

public class ResearchResponse
{
    public string Summary { get; set; } = string.Empty;
}

public class DuckDuckGoResponse
{
    public string Abstract { get; set; } = string.Empty;
    public string AbstractText { get; set; } = string.Empty;
    public List<RelatedTopic> RelatedTopics { get; set; } = new();
}

public class RelatedTopic
{
    public string Text { get; set; } = string.Empty;
}