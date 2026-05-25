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
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLesson(Guid id)
    {
        var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(nameIdentifierClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user identification in token." });
        }

        try
        {
            var data = await _lessonService.GetLessonByIdAsync(id, userId);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while loading lesson.", details = ex.Message });
        }
    }
}
