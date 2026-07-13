using OverLoad.Domain.Common;
using OverLoad.Domain.Enums;
using System;

namespace OverLoad.Domain.Entities;

public class BugReport : BaseEntity
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public int? LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BugReportStatus Status { get; set; } = BugReportStatus.Open;
    public string? InstructorNote { get; set; }
    public string? AdminNote { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? AttachmentUrl { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Lesson? Lesson { get; set; }
}
