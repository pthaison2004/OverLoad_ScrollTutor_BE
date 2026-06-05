using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.Implementations;
using OverLoad.Services.Interfaces;

namespace OverLoad.API.Controllers;

/// <summary>Manage courses on the platform.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILessonService _lessonService;
    private readonly IEnrollmentService _enrollmentService;

    public CoursesController(
    ICourseService courseService,
    ILessonService lessonService,
    IEnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _lessonService = lessonService;
        _enrollmentService = enrollmentService;
    }

    /// <summary>Get a paginated list of courses with filtering and sorting.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CourseQueryParams query)
        => Ok(await _courseService.GetAllAsync(query));

    /// <summary>Get a single course by ID, including its lessons.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _courseService.GetByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Get a course by its URL slug.</summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _courseService.GetBySlugAsync(slug);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Get all lessons of a course with progress if authenticated.</summary>
    [HttpGet("{id:int}/lessons")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLessons(int id)
    {
        int? userId = null;
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                       ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedId))
            userId = parsedId;

        var result = await _lessonService.GetByCourseIdWithProgressAsync(id, userId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Create a new course. Slug is auto-generated from title.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _courseService.CreateAsync(request);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>Update an existing course.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _courseService.UpdateAsync(id, request);
        if (!result.Success)
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return Ok(result);
    }

    /// <summary>Delete a course and all its lessons.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _courseService.DeleteAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Get courses by category.</summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCategory(
        string category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _courseService.GetByCategoryAsync(category, page, pageSize);
        return Ok(result);
    }
    [HttpGet("{courseId:int}/progress")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseProgress(int courseId)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                       ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Invalid token." });

        var result = await _enrollmentService.GetCourseProgressAsync(userId, courseId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}
