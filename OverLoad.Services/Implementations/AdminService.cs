using OverLoad.Domain.Enums;
using OverLoad.Repositories.Interfaces;
using OverLoad.Services.Common;
using OverLoad.Services.DTOs.Response;
using OverLoad.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLoad.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public AdminService(
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<ApiResponse<AdminDashboardResponse>> GetDashboardAsync()
        {
            var (users, totalUsers) = await _userRepository.SearchAsync(null, null, 1, 1000, null, false);
            var (courses, totalCourses) = await _courseRepository.SearchAsync(null, null, null, null, 1, 1000, null, false);
            var enrollments = await _enrollmentRepository.GetAllAsync();

            var recentUsers = users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new RecentUserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    CreatedAt = u.CreatedAt
                }).ToList();

            var usersByRole = users
                .GroupBy(u => u.Role.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            return ApiResponse<AdminDashboardResponse>.SuccessResult(new AdminDashboardResponse
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalEnrollments = enrollments.Count(),
                UsersByRole = usersByRole,
                RecentUsers = recentUsers
            });
        }

        public async Task<ApiResponse<bool>> ChangeRoleAsync(int userId, string role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailResult("User not found.");

            if (!Enum.TryParse<UserRole>(role, true, out var newRole))
                return ApiResponse<bool>.FailResult($"Invalid role: {role}");

            if (newRole == UserRole.Admin)
                return ApiResponse<bool>.FailResult("Cannot assign Admin role through this endpoint.");

            user.Role = newRole;
            await _userRepository.UpdateAsync(user);
            return ApiResponse<bool>.SuccessResult(true, $"Role changed to {role}.");
        }

        public async Task<ApiResponse<bool>> ToggleLockAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailResult("User not found.");

            user.IsLocked = !user.IsLocked;
            await _userRepository.UpdateAsync(user);

            var msg = user.IsLocked ? "User locked." : "User unlocked.";
            return ApiResponse<bool>.SuccessResult(true, msg);
        }
    }
}
