using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overload.BusinessLogic.Interfaces;

namespace Overload.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILessonService _lessonService;

    public CoursesController(ICourseService courseService, ILessonService lessonService)
    {
        _courseService = courseService;
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var data = await _courseService.GetCoursesAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var courses = await _courseService.GetCoursesAsync();
        var course = courses.FirstOrDefault(c => c.Id == id);
        if (course == null)
        {
            return NotFound(new { message = "Course not found." });
        }
        return Ok(course);
    }

    [Authorize]
    [HttpGet("{id}/lessons")]
    public async Task<IActionResult> GetLessons(Guid id)
    {
        var data = await _lessonService.GetLessonsByCourseIdAsync(id);
        return Ok(data);
    }
}
