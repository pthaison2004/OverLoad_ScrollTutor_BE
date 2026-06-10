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
    public class ManagerService : IManagerService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public ManagerService(
            ICourseRepository courseRepository,
            ILessonRepository lessonRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<ApiResponse<ManagerDashboardResponse>> GetDashboardAsync()
        {
            var (courses, totalCourses) = await _courseRepository.SearchAsync(null, null, null, null, 1, 1000, null, false);
            var enrollments = await _enrollmentRepository.GetAllAsync();
            var courseList = courses.ToList();

            var topCourses = courseList
                .Select(c => new CourseEnrollmentStats
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    IsPublished = c.IsPublished,
                    EnrollmentCount = enrollments.Count(e => e.CourseId == c.Id)
                })
                .OrderByDescending(c => c.EnrollmentCount)
                .Take(5)
                .ToList();

            var totalLessons = 0;
            foreach (var course in courseList)
            {
                var lessons = await _lessonRepository.GetByCourseIdAsync(course.Id);
                totalLessons += lessons.Count();
            }

            return ApiResponse<ManagerDashboardResponse>.SuccessResult(new ManagerDashboardResponse
            {
                TotalCourses = totalCourses,
                TotalLessons = totalLessons,
                TotalEnrollments = enrollments.Count(),
                PublishedCourses = courseList.Count(c => c.IsPublished),
                UnpublishedCourses = courseList.Count(c => !c.IsPublished),
                TopCourses = topCourses
            });
        }
    }
}
