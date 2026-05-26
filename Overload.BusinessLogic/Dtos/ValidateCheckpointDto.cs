using System;
using System.ComponentModel.DataAnnotations;

namespace Overload.BusinessLogic.Dtos;

/// <summary>
/// Request DTO for validating a checkpoint answer.
/// </summary>
public class ValidateCheckpointDto
{
    [Required]
    public Guid LessonId { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int CheckpointIndex { get; set; }

    [Required]
    public string UserAnswer { get; set; } = null!;
}

/// <summary>
/// Response DTO for checkpoint validation result.
/// </summary>
public class CheckpointResultDto
{
    public bool IsCorrect { get; set; }

    public string Message { get; set; } = null!;

    public int? NewUnlockedCheckpointIndex { get; set; }
}
