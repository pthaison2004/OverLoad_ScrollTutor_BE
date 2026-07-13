using System;

namespace OverLoad.Services.DTOs.Response;

public class BugReportResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int? LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? InstructorNote { get; set; }
    public string? AdminNote { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AttachmentUrl { get; set; }
}
