using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.Interfaces;

namespace OverLoad.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        public AdminController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }

        /// <summary>Dashboard thống kê tổng quan.</summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return Ok(result);
        }

        /// <summary>Danh sách users.</summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryParams query)
        {
            var result = await _userService.GetAllAsync(query);
            return Ok(result);
        }

        /// <summary>Đổi role user.</summary>
        [HttpPatch("users/{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleRequest request)
        {
            var result = await _adminService.ChangeRoleAsync(id, request.Role);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Khóa/mở tài khoản user.</summary>
        [HttpPatch("users/{id}/lock")]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var result = await _adminService.ToggleLockAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
