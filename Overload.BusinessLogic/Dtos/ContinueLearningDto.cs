using System;

namespace Overload.BusinessLogic.Dtos;

public class ContinueLearningDto
{
    public Guid CourseId { get; set; }

    public string CourseTitle { get; set; } = null!;

    public decimal ProgressPercentage { get; set; }

    public Guid LessonId { get; set; }

    public string LessonTitle { get; set; } = null!;

    public int OrderIndex { get; set; }

    public decimal LastScrollPercentage { get; set; }

    public int UnlockedCheckpointIndex { get; set; }
}
