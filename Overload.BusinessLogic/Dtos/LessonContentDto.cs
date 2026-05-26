using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Overload.BusinessLogic.Dtos;

/// <summary>
/// Represents the full lesson content with interactive steps,
/// served by GET /api/lessons/{id}/steps.
/// </summary>
public class LessonContentDto
{
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("total_steps")]
    public int TotalSteps { get; set; }

    [JsonPropertyName("steps")]
    public List<LessonStepDto> Steps { get; set; } = new();
}

/// <summary>
/// Represents a single interactive step within a lesson.
/// </summary>
public class LessonStepDto
{
    [JsonPropertyName("step_index")]
    public int StepIndex { get; set; }

    [JsonPropertyName("trigger_percentage")]
    public decimal TriggerPercentage { get; set; }

    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = null!;

    [JsonPropertyName("code_action")]
    public string CodeAction { get; set; } = null!;

    [JsonPropertyName("code_snippet")]
    public string CodeSnippet { get; set; } = null!;

    [JsonPropertyName("ui_render_state")]
    public UiRenderStateDto? UiRenderState { get; set; }

    [JsonPropertyName("checkpoint")]
    public CheckpointDto? Checkpoint { get; set; }
}

/// <summary>
/// Represents the UI render state for a lesson step.
/// </summary>
public class UiRenderStateDto
{
    [JsonPropertyName("component")]
    public string Component { get; set; } = null!;

    [JsonPropertyName("props")]
    public Dictionary<string, object>? Props { get; set; }
}

/// <summary>
/// Represents a checkpoint (quiz/challenge) embedded in a lesson step.
/// </summary>
public class CheckpointDto
{
    [JsonPropertyName("checkpoint_index")]
    public int CheckpointIndex { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("question")]
    public string Question { get; set; } = null!;

    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }

    [JsonPropertyName("correct_answer")]
    public string CorrectAnswer { get; set; } = null!;
}
