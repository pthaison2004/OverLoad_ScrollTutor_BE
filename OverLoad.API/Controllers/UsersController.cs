using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.Implementations;
using OverLoad.Services.Interfaces;
using OverLoad.Repositories.Data;

namespace OverLoad.API.Controllers;

/// <summary>Manage platform users.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public UsersController(IUserService userService, IEnrollmentService enrollmentService, AppDbContext context, IWebHostEnvironment env)
    {
        _userService = userService;
        _enrollmentService = enrollmentService;
        _context = context;
        _env = env;
    }

    /// <summary>Get a paginated, searchable list of users.</summary>
    /// <param name="query">Pagination, search, filter, and sort parameters.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryParams query)
    {
        var result = await _userService.GetAllAsync(query);
        return Ok(result);
    }

    /// <summary>Get a single user by ID (includes enrollment history).</summary>
    /// <param name="id">User ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Create a new user.</summary>
    /// <param name="request">User creation payload.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.CreateAsync(request);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>Update an existing user.</summary>
    /// <param name="id">User ID.</param>
    /// <param name="request">User update payload.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.UpdateAsync(id, request);
        if (!result.Success)
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return Ok(result);
    }

    /// <summary>Delete a user by ID.</summary>
    /// <param name="id">User ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
    /// <summary>Get all courses the authenticated user has enrolled in with progress.</summary>
    [HttpGet("me/courses")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyCourses()
    {
        // Lấy userId từ JWT token
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                       ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Invalid token." });

        var result = await _enrollmentService.GetMyCoursesAsync(userId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Upload student card for verification.</summary>
    [HttpPost("me/student-verification")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadStudentVerification(IFormFile file)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                       ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Invalid token." });

        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "Vui lòng chọn ảnh làm bằng chứng." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { success = false, message = "Định dạng file không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG, WEBP." });

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound(new { success = false, message = "Không tìm thấy tài khoản người dùng." });

        // Delete old student card image file from disk if it exists
        if (!string.IsNullOrEmpty(user.StudentCardPath))
        {
            var oldFilePath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), user.StudentCardPath.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath))
            {
                try
                {
                    System.IO.File.Delete(oldFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting old student card file {oldFilePath}: {ex.Message}");
                }
            }
        }

        // Save file
        var uploadDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "student_cards");
        if (!Directory.Exists(uploadDir))
        {
            Directory.CreateDirectory(uploadDir);
        }

        var fileName = $"student_{userId}_{DateTime.UtcNow.Ticks}{extension}";
        var filePath = Path.Combine(uploadDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save relative path
        user.StudentCardPath = $"/uploads/student_cards/{fileName}";
        user.StudentVerificationStatus = "PENDING";
        user.HasSeenStudentRejection = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { 
            success = true, 
            message = "Tải lên ảnh xác minh thành công. Đang chờ kiểm duyệt.",
            studentVerificationStatus = user.StudentVerificationStatus,
            studentCardPath = user.StudentCardPath
        });
    }

    /// <summary>Dismiss student verification rejection notice.</summary>
    [HttpPost("me/dismiss-rejection")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DismissRejection()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                       ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Invalid token." });

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound(new { success = false, message = "User not found." });

        user.HasSeenStudentRejection = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Đã ghi nhận thông báo từ chối." });
    }

    /// <summary>Get list of pending student verification requests (Admin/Instructor only).</summary>
    [HttpGet("student-verifications/pending")]
    [Authorize]
    public async Task<IActionResult> GetPendingStudentVerifications()
    {
        var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)
                     ?? User.FindFirst("role");

        if (roleClaim == null || (roleClaim.Value != "Admin" && roleClaim.Value != "Instructor"))
            return Forbid();

        var pendingUsers = await _context.Users
            .Where(u => u.StudentVerificationStatus == "PENDING")
            .OrderByDescending(u => u.UpdatedAt)
            .Select(u => new {
                id = u.Id,
                fullName = u.FullName,
                email = u.Email,
                avatarUrl = u.AvatarUrl,
                studentCardPath = u.StudentCardPath,
                updatedAt = u.UpdatedAt
            })
            .ToListAsync();

        return Ok(pendingUsers);
    }

    /// <summary>Get list of approved student verifications (Admin/Instructor only).</summary>
    [HttpGet("student-verifications/approved")]
    [Authorize]
    public async Task<IActionResult> GetApprovedStudentVerifications()
    {
        var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)
                     ?? User.FindFirst("role");

        if (roleClaim == null || (roleClaim.Value != "Admin" && roleClaim.Value != "Instructor"))
            return Forbid();

        var approvedUsers = await _context.Users
            .Where(u => u.StudentVerificationStatus == "APPROVED")
            .OrderByDescending(u => u.UpdatedAt)
            .Select(u => new {
                id = u.Id,
                fullName = u.FullName,
                email = u.Email,
                avatarUrl = u.AvatarUrl,
                studentCardPath = u.StudentCardPath,
                updatedAt = u.UpdatedAt
            })
            .ToListAsync();

        return Ok(approvedUsers);
    }

    public class VerifyStudentRequest
    {
        public int UserId { get; set; }
        public string Action { get; set; } = string.Empty; // approve or reject
    }

    /// <summary>Approve or reject a student verification request (Admin/Instructor only).</summary>
    [HttpPost("student-verification/verify")]
    [Authorize]
    public async Task<IActionResult> VerifyStudent([FromBody] VerifyStudentRequest request)
    {
        var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)
                     ?? User.FindFirst("role");

        if (roleClaim == null || (roleClaim.Value != "Admin" && roleClaim.Value != "Instructor"))
            return Forbid();

        if (request == null || (request.Action != "approve" && request.Action != "reject"))
            return BadRequest(new { success = false, message = "Hành động không hợp lệ. Chỉ chấp nhận 'approve' hoặc 'reject'." });

        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return NotFound(new { success = false, message = "Không tìm thấy người dùng." });

        if (request.Action == "approve")
        {
            user.StudentVerificationStatus = "APPROVED";
            user.HasSeenStudentRejection = false;
        }
        else // reject
        {
            user.StudentVerificationStatus = "REJECTED";
            user.HasSeenStudentRejection = false;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { 
            success = true, 
            message = $"Đã {(request.Action == "approve" ? "phê duyệt" : "từ chối")} xác minh sinh viên thành công.",
            studentVerificationStatus = user.StudentVerificationStatus
        });
    }
}
