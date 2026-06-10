using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLoad.Services.DTOs.Response
{
    public class AdminDashboardResponse
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public List<RecentUserResponse> RecentUsers { get; set; } = new();
    }

    public class ManagerDashboardResponse
    {
        public int TotalCourses { get; set; }
        public int TotalLessons { get; set; }
        public int TotalEnrollments { get; set; }
        public int PublishedCourses { get; set; }
        public int UnpublishedCourses { get; set; }
        public List<CourseEnrollmentStats> TopCourses { get; set; } = new();
    }

    public class RecentUserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CourseEnrollmentStats
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
        public bool IsPublished { get; set; }
    }
}
