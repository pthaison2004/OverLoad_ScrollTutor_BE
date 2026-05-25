using System;

namespace Overload.BusinessLogic.Dtos;

public class LessonDto
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Content { get; set; }

    public int? DurationMinutes { get; set; }

    public int OrderIndex { get; set; }

    public bool IsFree { get; set; }
}
