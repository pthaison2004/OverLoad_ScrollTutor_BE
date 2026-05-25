using System;

namespace Overload.BusinessLogic.Dtos;

public class EnrollmentDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CourseId { get; set; }

    public string CourseTitle { get; set; } = null!;

    public string? CourseDescription { get; set; }

    public DateTime? EnrolledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public decimal ProgressPercentage { get; set; }

    public DateTime? LastAccessedAt { get; set; }
}
