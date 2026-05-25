using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overload.BusinessLogic.Dtos;
using Overload.BusinessLogic.Interfaces;

namespace Overload.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpPost("update-scroll")]
    public async Task<IActionResult> UpdateScroll([FromBody] UpdateScrollDto dto)
    {
        var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(nameIdentifierClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user identification in token." });
        }

        try
        {
            await _progressService.UpdateScrollAsync(userId, dto);
            return Ok(new { message = "Progress updated successfully.", userId = userId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating progress.", details = ex.Message });
        }
    }

    [HttpGet("continue-learning")]
    public async Task<IActionResult> GetContinueLearning()
    {
        var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(nameIdentifierClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user identification in token." });
        }

        try
        {
            var result = await _progressService.GetContinueLearningAsync(userId);
            if (result == null)
            {
                return Ok(new { message = "No active course in progress." });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while calculating continuation details.", details = ex.Message });
        }
    }
}
