using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.Interfaces;
namespace OverLoad.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IManagerService _managerService;

        public ManagerController(
            ICourseService courseService,
            ILessonService lessonService,
            IEnrollmentService enrollmentService,
            IManagerService managerService)
        {
            _courseService = courseService;
            _lessonService = lessonService;
            _enrollmentService = enrollmentService;
            _managerService = managerService;
        }

        // ── Dashboard ─────────────────────────────────────────────────────────────
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _managerService.GetDashboardAsync();
            return Ok(result);
        }

        // ── Courses ───────────────────────────────────────────────────────────────
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] CourseQueryParams query)
        {
            var result = await _courseService.GetAllAsync(query);
            return Ok(result);
        }

        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
        {
            var result = await _courseService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return StatusCode(201, result);
        }

        [HttpPut("courses/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
        {
            var result = await _courseService.UpdateAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("courses/{id}/publish")]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var result = await _courseService.TogglePublishAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // ── Lessons ───────────────────────────────────────────────────────────────
        [HttpGet("courses/{courseId}/lessons")]
        public async Task<IActionResult> GetLessons(int courseId)
        {
            var result = await _lessonService.GetByCourseIdAsync(courseId);
            return Ok(result);
        }

        [HttpPost("courses/{courseId}/lessons")]
        public async Task<IActionResult> CreateLesson(int courseId, [FromBody] CreateLessonRequest request)
        {
            request.CourseId = courseId;
            var result = await _lessonService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return StatusCode(201, result);
        }

        [HttpPut("lessons/{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] UpdateLessonRequest request)
        {
            var result = await _lessonService.UpdateAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("lessons/{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var result = await _lessonService.DeleteAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // ── Enrollments ───────────────────────────────────────────────────────────
        [HttpGet("enrollments")]
        public async Task<IActionResult> GetEnrollments()
        {
            var result = await _enrollmentService.GetAllAsync(new EnrollmentQueryParams { PageSize = 1000 });
            return Ok(result);
        }

        [HttpDelete("enrollments/{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var result = await _enrollmentService.UnenrollAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
