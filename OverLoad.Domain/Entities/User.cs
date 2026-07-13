using OverLoad.Domain.Common;
using OverLoad.Domain.Enums;

namespace OverLoad.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsVerified { get; set; } = false;
    public string StudentVerificationStatus { get; set; } = "NONE";
    public string? StudentCardPath { get; set; }
    public bool HasSeenStudentRejection { get; set; } = false;
    public bool IsLocked { get; set; } = false;
    // JWT Refresh Token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    
    // AI Chatbot Limits
    public int AiQuestionsAskedToday { get; set; } = 0;
    public DateTime LastAiQuestionDate { get; set; } = DateTime.UtcNow;


    // Navigation
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<UserLessonProgress> LessonProgresses { get; set; } = new List<UserLessonProgress>();
    public ICollection<BugReport> BugReports { get; set; } = new List<BugReport>();
}
