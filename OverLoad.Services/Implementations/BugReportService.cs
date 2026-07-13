using OverLoad.Domain.Entities;
using OverLoad.Domain.Enums;
using OverLoad.Repositories.Interfaces;
using OverLoad.Services.Common;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.DTOs.Response;
using OverLoad.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverLoad.Services.Implementations;

public class BugReportService : IBugReportService
{
    private readonly IBugReportRepository _bugReportRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IUserRepository _userRepository;

    public BugReportService(
        IBugReportRepository bugReportRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        IUserRepository userRepository)
    {
        _bugReportRepository = bugReportRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<BugReportResponse>> CreateAsync(int userId, CreateBugReportRequest request)
    {
        if (!await _userRepository.ExistsAsync(userId))
            return ApiResponse<BugReportResponse>.FailResult("User not found.");

        if (!await _courseRepository.ExistsAsync(request.CourseId))
            return ApiResponse<BugReportResponse>.FailResult("Course not found.");

        if (request.LessonId.HasValue && !await _lessonRepository.ExistsAsync(request.LessonId.Value))
            return ApiResponse<BugReportResponse>.FailResult("Lesson not found.");

        var bugReport = new BugReport
        {
            UserId = userId,
            CourseId = request.CourseId,
            LessonId = request.LessonId,
            Title = request.Title,
            Description = request.Description,
            Status = BugReportStatus.Open,
            AttachmentUrl = request.AttachmentUrl
        };

        await _bugReportRepository.AddAsync(bugReport);
        var created = await _bugReportRepository.GetByIdWithDetailsAsync(bugReport.Id);
        return ApiResponse<BugReportResponse>.SuccessResult(MapToResponse(created!), "Báo cáo lỗi đã được gửi thành công.");
    }

    public async Task<ApiResponse<BugReportResponse>> UpdateStatusAsync(int id, UpdateBugReportStatusRequest request)
    {
        var existing = await _bugReportRepository.GetByIdAsync(id);
        if (existing == null)
            return ApiResponse<BugReportResponse>.FailResult("Bug report not found.");

        if (!Enum.TryParse<BugReportStatus>(request.Status, true, out var status))
            return ApiResponse<BugReportResponse>.FailResult("Trạng thái không hợp lệ.");

        existing.Status = status;
        if (status == BugReportStatus.Resolved || status == BugReportStatus.Closed)
        {
            existing.ResolvedAt = DateTime.UtcNow;
        }
        else
        {
            existing.ResolvedAt = null;
        }

        existing.InstructorNote = request.InstructorNote;
        existing.AdminNote = request.AdminNote;
        existing.UpdatedAt = DateTime.UtcNow;

        await _bugReportRepository.UpdateAsync(existing);
        var updated = await _bugReportRepository.GetByIdWithDetailsAsync(id);
        return ApiResponse<BugReportResponse>.SuccessResult(MapToResponse(updated!), "Cập nhật trạng thái thành công.");
    }

    public async Task<ApiResponse<List<BugReportResponse>>> GetByCourseIdAsync(int courseId)
    {
        var list = await _bugReportRepository.GetByCourseIdWithDetailsAsync(courseId);
        var result = list.Select(MapToResponse).ToList();
        return ApiResponse<List<BugReportResponse>>.SuccessResult(result);
    }

    public async Task<ApiResponse<List<BugReportResponse>>> GetByUserIdAsync(int userId)
    {
        var list = await _bugReportRepository.GetByUserIdWithDetailsAsync(userId);
        var result = list.Select(MapToResponse).ToList();
        return ApiResponse<List<BugReportResponse>>.SuccessResult(result);
    }

    public async Task<PagedResponse<BugReportResponse>> SearchAsync(BugReportQueryParams queryParams)
    {
        BugReportStatus? status = null;
        if (!string.IsNullOrWhiteSpace(queryParams.Status) && Enum.TryParse<BugReportStatus>(queryParams.Status, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var (items, total) = await _bugReportRepository.SearchAsync(
            queryParams.Page, queryParams.PageSize, queryParams.CourseId, status, queryParams.Category, queryParams.SearchTerm);

        var responseItems = items.Select(MapToResponse).ToList();
        return PagedResponse<BugReportResponse>.SuccessResult(responseItems, total, queryParams.Page, queryParams.PageSize);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        if (!await _bugReportRepository.ExistsAsync(id))
            return ApiResponse<bool>.FailResult("Bug report not found.");

        await _bugReportRepository.DeleteAsync(id);
        return ApiResponse<bool>.SuccessResult(true, "Báo cáo lỗi đã được xóa.");
    }

    private static BugReportResponse MapToResponse(BugReport r) => new()
    {
        Id = r.Id,
        UserId = r.UserId,
        UserFullName = r.User?.FullName ?? string.Empty,
        UserEmail = r.User?.Email ?? string.Empty,
        CourseId = r.CourseId,
        CourseTitle = r.Course?.Title ?? string.Empty,
        LessonId = r.LessonId,
        LessonTitle = r.Lesson?.Title,
        Title = r.Title,
        Description = r.Description,
        Status = r.Status.ToString(),
        InstructorNote = r.InstructorNote,
        AdminNote = r.AdminNote,
        ResolvedAt = r.ResolvedAt,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        AttachmentUrl = r.AttachmentUrl
    };
}
