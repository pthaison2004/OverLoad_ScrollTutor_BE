using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.Implementations;
using OverLoad.Services.Interfaces;

namespace OverLoad.API.Controllers;

/// <summary>Manage platform users.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEnrollmentService _enrollmentService;
    public UsersController(IUserService userService, IEnrollmentService enrollmentService)
    {
        _userService = userService;
        _enrollmentService = enrollmentService;
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
}
