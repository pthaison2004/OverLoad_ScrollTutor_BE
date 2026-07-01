using OverLoad.Domain.Entities;
using OverLoad.Repositories.Interfaces;
using OverLoad.Services.Common;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.DTOs.Response;
using OverLoad.Services.Interfaces;

namespace OverLoad.Services.Implementations;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserLessonProgressRepository _progressRepository;
    private readonly IUserRepository _userRepository;

    public LessonService(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IUserLessonProgressRepository progressRepository,
        IUserRepository userRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _progressRepository = progressRepository;
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<LessonResponse>> GetByIdAsync(int id)
    {
        var lesson = await _lessonRepository.GetWithCourseAsync(id);
        if (lesson == null)
            return ApiResponse<LessonResponse>.FailResult("Lesson not found.");

        return ApiResponse<LessonResponse>.SuccessResult(MapToResponse(lesson));
    }

    public async Task<PagedResponse<LessonResponse>> GetAllAsync(LessonQueryParams query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Clamp(query.PageSize, 1, 100);

        var (items, total) = await _lessonRepository.SearchAsync(
            query.CourseId, query.Search, query.IsFree,
            query.Page, query.PageSize, query.SortBy, query.SortDesc);

        // Load course info for mapping
        var lessonList = new List<LessonResponse>();
        foreach (var lesson in items)
        {
            var withCourse = await _lessonRepository.GetWithCourseAsync(lesson.Id);
            if (withCourse != null) lessonList.Add(MapToResponse(withCourse));
        }

        return PagedResponse<LessonResponse>.SuccessResult(lessonList, total, query.Page, query.PageSize);
    }

    public async Task<ApiResponse<List<LessonResponse>>> GetByCourseIdAsync(int courseId)
    {
        if (!await _courseRepository.ExistsAsync(courseId))
            return ApiResponse<List<LessonResponse>>.FailResult("Course not found.");

        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        var responses = lessons.Select(l => new LessonResponse
        {
            Id = l.Id,
            CourseId = l.CourseId,
            CourseTitle = string.Empty,
            Title = l.Title,
            Description = l.Description,
            Content = l.Content,
            DurationMinutes = l.DurationMinutes,
            OrderIndex = l.OrderIndex,
            IsFree = l.IsFree,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        }).ToList();

        return ApiResponse<List<LessonResponse>>.SuccessResult(responses);
    }

    public async Task<ApiResponse<LessonResponse>> CreateAsync(CreateLessonRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            return ApiResponse<LessonResponse>.FailResult("Course not found.");

        var orderIndex = await _lessonRepository.GetNextOrderIndexAsync(request.CourseId);

        var lesson = new Lesson
        {
            CourseId = request.CourseId,
            Title = request.Title.Trim(),
            Description = request.Description,
            Content = request.Content,
            DurationMinutes = request.DurationMinutes,
            OrderIndex = orderIndex,
            IsFree = request.IsFree
        };

        var created = await _lessonRepository.AddAsync(lesson);

        // Sync course totals
        await SyncCourseTotalsAsync(course, request.CourseId);

        created.Course = course;
        return ApiResponse<LessonResponse>.SuccessResult(MapToResponse(created), "Lesson created successfully.");
    }

    public async Task<ApiResponse<LessonResponse>> UpdateAsync(int id, UpdateLessonRequest request)
    {
        var lesson = await _lessonRepository.GetWithCourseAsync(id);
        if (lesson == null)
            return ApiResponse<LessonResponse>.FailResult("Lesson not found.");

        lesson.Title = request.Title.Trim();
        lesson.Description = request.Description;
        lesson.Content = request.Content;
        lesson.DurationMinutes = request.DurationMinutes;
        if (request.OrderIndex.HasValue)
        {
            lesson.OrderIndex = request.OrderIndex.Value;
        }
        lesson.IsFree = request.IsFree;

        await _lessonRepository.UpdateAsync(lesson);

        // Sync course totals
        var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
        if (course != null) await SyncCourseTotalsAsync(course, lesson.CourseId);

        return ApiResponse<LessonResponse>.SuccessResult(MapToResponse(lesson), "Lesson updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            return ApiResponse<bool>.FailResult("Lesson not found.");

        var courseId = lesson.CourseId;
        await _lessonRepository.DeleteAsync(id);

        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course != null) await SyncCourseTotalsAsync(course, courseId);

        return ApiResponse<bool>.SuccessResult(true, "Lesson deleted successfully.");
    }
    public async Task<ApiResponse<List<LessonProgressResponse>>> GetByCourseIdWithProgressAsync(int courseId, int? userId)
    {
        if (!await _courseRepository.ExistsAsync(courseId))
            return ApiResponse<List<LessonProgressResponse>>.FailResult("Course not found.");

        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        var lessonList = lessons.OrderBy(l => l.OrderIndex).ToList();

        // Lấy progress nếu user đã đăng nhập
        List<UserLessonProgress> progressList = new();
        bool isStaff = false;

        if (userId.HasValue)
        {
            var allProgress = await _progressRepository.GetByUserIdAsync(userId.Value);
            progressList = allProgress
                .Where(p => lessonList.Select(l => l.Id).Contains(p.LessonId))
                .ToList();

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user != null)
            {
                var roleStr = user.Role.ToString();
                isStaff = roleStr == "Admin" || roleStr == "Instructor" || roleStr == "Manager";
            }
        }

        var result = lessonList.Select((lesson, index) =>
        {
            var progress = progressList.FirstOrDefault(p => p.LessonId == lesson.Id);

            // Gói học tuần tự: bài học đầu tiên (index = 0) luôn mở khóa.
            // Các bài học tiếp theo bị khóa trừ khi người dùng là Admin/Instructor/Manager hoặc đã hoàn thành bài trước đó.
            bool isLocked = false;
            if (index > 0 && !isStaff)
            {
                var previousLesson = lessonList[index - 1];
                var previousCompleted = progressList.Any(p => p.LessonId == previousLesson.Id && p.Completed);
                isLocked = !previousCompleted;
            }

            var watchPercentage = progress != null && lesson.DurationMinutes > 0
                ? Math.Clamp(
                    Math.Round((decimal)progress.LastPositionSeconds / (lesson.DurationMinutes * 60) * 100, 1),
                    0, 100)
                : 0;

            return new LessonProgressResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                Content = lesson.Content,
                DurationMinutes = lesson.DurationMinutes,
                OrderIndex = lesson.OrderIndex,
                IsFree = lesson.IsFree,
                Completed = progress?.Completed ?? false,
                WatchPercentage = watchPercentage,
                LastPositionSeconds = progress?.LastPositionSeconds ?? 0,
                IsLocked = isLocked
            };
        }).ToList();

        return ApiResponse<List<LessonProgressResponse>>.SuccessResult(result);
    }
    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task SyncCourseTotalsAsync(Course course, int courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        var lessonList = lessons.ToList();
        course.TotalLessons = lessonList.Count;
        course.TotalDurationMinutes = lessonList.Sum(l => l.DurationMinutes);
        await _courseRepository.UpdateAsync(course);
    }

    private static LessonResponse MapToResponse(Lesson l) => new()
    {
        Id = l.Id,
        CourseId = l.CourseId,
        CourseTitle = l.Course?.Title ?? string.Empty,
        Title = l.Title,
        Description = l.Description,
        Content = l.Content,
        DurationMinutes = l.DurationMinutes,
        OrderIndex = l.OrderIndex,
        IsFree = l.IsFree,
        CreatedAt = l.CreatedAt,
        UpdatedAt = l.UpdatedAt
    };
}
