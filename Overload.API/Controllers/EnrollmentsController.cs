using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overload.BusinessLogic.Interfaces;

namespace Overload.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpPost("enroll/{courseId}")]
    public async Task<IActionResult> Enroll(Guid courseId)
    {
        var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(nameIdentifierClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user identification in token." });
        }

        try
        {
            var data = await _enrollmentService.EnrollAsync(userId, courseId);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while enrolling in course.", details = ex.Message });
        }
    }

    [HttpGet("my-enrollments")]
    public async Task<IActionResult> GetMyEnrollments()
    {
        var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(nameIdentifierClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user identification in token." });
        }

        try
        {
            var data = await _enrollmentService.GetMyEnrollmentsAsync(userId);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while loading enrollments.", details = ex.Message });
        }
    }
}
