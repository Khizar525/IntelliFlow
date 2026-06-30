using FluentAssertions;
using Xunit;

namespace Orchestrator.Tests;

/// <summary>
/// Tests for orchestrator pipeline logic.
/// </summary>
public class OrchestratorLogicTests
{
    [Fact]
    public void GenerateTaskId_ShouldReturnUniqueIds()
    {
        // Arrange & Act
        var id1 = GenerateTaskId();
        var id2 = GenerateTaskId();

        // Assert
        id1.Should().NotBeNullOrEmpty();
        id2.Should().NotBeNullOrEmpty();
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void GenerateTaskId_ShouldBeValidGuid()
    {
        // Arrange & Act
        var taskId = GenerateTaskId();

        // Assert
        Guid.TryParse(taskId, out _).Should().BeTrue();
    }

    [Fact]
    public void PipelineStatus_WhenStarted_ShouldBeProcessing()
    {
        // Arrange
        var status = new PipelineStatus();

        // Act
        status.Start();

        // Assert
        status.CurrentStatus.Should().Be("Processing");
        status.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void PipelineStatus_WhenCompleted_ShouldBeSuccess()
    {
        // Arrange
        var status = new PipelineStatus();

        // Act
        status.Start();
        status.Complete();

        // Assert
        status.CurrentStatus.Should().Be("Completed");
        status.EndTime.Should().HaveValue();
    }

    [Fact]
    public void PipelineStatus_WhenFailed_ShouldBeError()
    {
        // Arrange
        var status = new PipelineStatus();

        // Act
        status.Start();
        status.Fail("Test error");

        // Assert
        status.CurrentStatus.Should().Be("Failed");
        status.ErrorMessage.Should().Be("Test error");
    }

    [Fact]
    public void PipelineStatus_Duration_ShouldCalculateCorrectly()
    {
        // Arrange
        var status = new PipelineStatus();
        status.Start();

        // Act
        Thread.Sleep(100); // Simulate work
        status.Complete();

        // Assert
        status.Duration.Should().BeGreaterThan(TimeSpan.Zero);
        status.Duration.Should().BeGreaterThan(TimeSpan.FromMilliseconds(50));
    }

    [Fact]
    public void TopicClassifier_TechnicalTopic_ShouldReturnTechnical()
    {
        // Arrange
        var topics = new[] { "Machine Learning", "Cloud Computing", "API Design", "Docker Kubernetes" };

        // Act & Assert
        foreach (var topic in topics)
        {
            var classification = ClassifyTopic(topic);
            classification.Should().Be("Technical");
        }
    }

    [Fact]
    public void TopicClassifier_GeneralTopic_ShouldReturnGeneral()
    {
        // Arrange
        var topics = new[] { "Cooking Recipes", "Travel Guide", "Fitness Tips", "Fashion Trends" };

        // Act & Assert
        foreach (var topic in topics)
        {
            var classification = ClassifyTopic(topic);
            classification.Should().Be("General");
        }
    }

    [Fact]
    public void ModelSelector_TechnicalTopic_ShouldSelectTechnicalModel()
    {
        // Arrange & Act
        var model = SelectModel("Technical");

        // Assert
        model.Should().Be("google/gemma-4-26b-a4b-it:free");
    }

    [Fact]
    public void ModelSelector_GeneralTopic_ShouldSelectGeneralModel()
    {
        // Arrange & Act
        var model = SelectModel("General");

        // Assert
        model.Should().Be("meta-llama/llama-3.2-3b-instruct:free");
    }

    // Helper methods
    private static string GenerateTaskId()
    {
        return Guid.NewGuid().ToString();
    }

    private static string ClassifyTopic(string topic)
    {
        var technicalKeywords = new[] { "machine", "learning", "cloud", "computing", "api", "docker", "kubernetes", "software", "programming", "ai", "data" };
        
        if (technicalKeywords.Any(k => topic.ToLower().Contains(k)))
        {
            return "Technical";
        }
        
        return "General";
    }

    private static string SelectModel(string classification)
    {
        return classification switch
        {
            "Technical" => "google/gemma-4-26b-a4b-it:free",
            "General" => "meta-llama/llama-3.2-3b-instruct:free",
            _ => "google/gemma-4-26b-a4b-it:free"
        };
    }
}

public class PipelineStatus
{
    public string CurrentStatus { get; private set; } = "Pending";
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public string? ErrorMessage { get; private set; }
    public TimeSpan Duration => (EndTime ?? DateTime.UtcNow) - (StartTime ?? DateTime.UtcNow);

    public void Start()
    {
        CurrentStatus = "Processing";
        StartTime = DateTime.UtcNow;
    }

    public void Complete()
    {
        CurrentStatus = "Completed";
        EndTime = DateTime.UtcNow;
    }

    public void Fail(string error)
    {
        CurrentStatus = "Failed";
        ErrorMessage = error;
        EndTime = DateTime.UtcNow;
    }
}
