using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OverLoad.API.Controllers;

[Authorize]
[ApiController]
[Route("api/bug-reports")]
[Produces("application/json")]
public class BugReportsController : ControllerBase
{
    private readonly IBugReportService _bugReportService;
    private readonly IWebHostEnvironment _env;

    public BugReportsController(IBugReportService bugReportService, IWebHostEnvironment env)
    {
        _bugReportService = bugReportService;
        _env = env;
    }

    /// <summary>Create a new bug report (Student, Instructor, Admin).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBugReportRequest request)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var result = await _bugReportService.CreateAsync(userId, request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Upload attachment image for a bug report.</summary>
    [HttpPost("upload-attachment")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAttachment(IFormFile file)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr))
            return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "Vui lòng chọn ảnh đính kèm." });

        if (file.Length > 5 * 1024 * 1024) // 5MB limit
            return BadRequest(new { success = false, message = "Ảnh không được vượt quá 5MB." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { success = false, message = "Định dạng file không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG, WEBP, GIF." });

        var uploadDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "bug_reports");
        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        var fileName = $"bug_{userIdStr}_{DateTime.UtcNow.Ticks}{extension}";
        var filePath = Path.Combine(uploadDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/bug_reports/{fileName}";
        return Ok(new { success = true, attachmentUrl = relativePath, message = "Tải ảnh lên thành công." });
    }

    /// <summary>Get bug reports reported by the authenticated student.</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReports()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var result = await _bugReportService.GetByUserIdAsync(userId);
        return Ok(result);
    }

    /// <summary>Get bug reports by Course ID (Instructor, Admin, Manager).</summary>
    [HttpGet("course/{courseId:int}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Instructors can only view reports if authenticated
        if (string.IsNullOrEmpty(userIdStr))
            return Unauthorized();

        var result = await _bugReportService.GetByCourseIdAsync(courseId);
        return Ok(result);
    }

    /// <summary>Get all bug reports with pagination and filter (Admin, Manager).</summary>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] BugReportQueryParams queryParams)
    {
        var result = await _bugReportService.SearchAsync(queryParams);
        return Ok(result);
    }

    /// <summary>Update bug report status (Instructor, Admin, Manager).</summary>
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBugReportStatusRequest request)
    {
        var result = await _bugReportService.UpdateStatusAsync(id, request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Delete a bug report (Admin only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _bugReportService.DeleteAsync(id);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
