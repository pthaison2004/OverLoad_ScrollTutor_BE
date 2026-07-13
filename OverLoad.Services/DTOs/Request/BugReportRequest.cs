using System.ComponentModel.DataAnnotations;

namespace OverLoad.Services.DTOs.Request;

public class CreateBugReportRequest
{
    [Required(ErrorMessage = "Mã khóa học là bắt buộc.")]
    public int CourseId { get; set; }

    public int? LessonId { get; set; }

    [Required(ErrorMessage = "Tiêu đề báo cáo là bắt buộc.")]
    [MinLength(5, ErrorMessage = "Tiêu đề phải có tối thiểu 5 ký tự.")]
    [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung mô tả lỗi là bắt buộc.")]
    [MinLength(10, ErrorMessage = "Mô tả phải có tối thiểu 10 ký tự.")]
    [MaxLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự.")]
    public string Description { get; set; } = string.Empty;

    public string? AttachmentUrl { get; set; }
}

public class UpdateBugReportStatusRequest
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    public string Status { get; set; } = string.Empty;

    public string? InstructorNote { get; set; }

    public string? AdminNote { get; set; }
}

public class BugReportQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? CourseId { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
}
