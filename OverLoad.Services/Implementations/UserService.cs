using Microsoft.EntityFrameworkCore;
using OverLoad.Domain.Entities;
using OverLoad.Domain.Enums;
using OverLoad.Repositories.Data;
using OverLoad.Repositories.Interfaces;
using OverLoad.Services.Common;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.DTOs.Response;
using OverLoad.Services.Interfaces;

namespace OverLoad.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _context;

    public UserService(IUserRepository userRepository, AppDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<ApiResponse<UserDetailResponse>> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetWithEnrollmentsAsync(id);
        if (user == null)
            return ApiResponse<UserDetailResponse>.FailResult("User not found.");

        var totalDeposited = await _context.Transactions
            .Where(t => t.UserId == id && t.Status == "SUCCESS" && t.Amount > 0)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        var balance = await _context.Transactions
            .Where(t => t.UserId == id && t.Status == "SUCCESS" && (t.CourseId == 6 || (t.Course != null && t.Course.Slug == "system-deposit-balance") || t.Amount < 0))
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        return ApiResponse<UserDetailResponse>.SuccessResult(MapToDetailResponse(user, totalDeposited, balance));
    }

    public async Task<PagedResponse<UserResponse>> GetAllAsync(UserQueryParams query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Clamp(query.PageSize, 1, 100);

        var (items, total) = await _userRepository.SearchAsync(
            query.Search, query.Role, query.Page, query.PageSize, query.SortBy, query.SortDesc);

        var userIds = items.Select(u => u.Id).ToList();

        var successTransactions = await _context.Transactions
            .Include(t => t.Course)
            .Where(t => userIds.Contains(t.UserId) && t.Status == "SUCCESS")
            .ToListAsync();

        var totalDepositedMap = successTransactions
            .Where(t => t.Amount > 0)
            .GroupBy(t => t.UserId)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        var balanceMap = successTransactions
            .Where(t => t.CourseId == 6 || (t.Course != null && t.Course.Slug == "system-deposit-balance") || t.Amount < 0)
            .GroupBy(t => t.UserId)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        var responses = items.Select(u => MapToResponse(
            u,
            totalDepositedMap.GetValueOrDefault(u.Id, 0m),
            balanceMap.GetValueOrDefault(u.Id, 0m)
        ));

        return PagedResponse<UserResponse>.SuccessResult(responses, total, query.Page, query.PageSize);
    }

    public async Task<ApiResponse<UserResponse>> CreateAsync(CreateUserRequest request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
            return ApiResponse<UserResponse>.FailResult("Email already in use.", "Duplicate email address.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            return ApiResponse<UserResponse>.FailResult("Invalid role specified.", $"Role '{request.Role}' is not valid.");

        var user = new User
        {
            Email = request.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName.Trim(),
            AvatarUrl = request.AvatarUrl,
            Bio = request.Bio,
            Role = role,
            IsVerified = false
        };

        var created = await _userRepository.AddAsync(user);
        return ApiResponse<UserResponse>.SuccessResult(MapToResponse(created), "User created successfully.");
    }

    public async Task<ApiResponse<UserResponse>> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return ApiResponse<UserResponse>.FailResult("User not found.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            return ApiResponse<UserResponse>.FailResult("Invalid role specified.", $"Role '{request.Role}' is not valid.");

        user.FullName = request.FullName.Trim();
        user.AvatarUrl = request.AvatarUrl;
        user.Bio = request.Bio;
        user.IsVerified = request.IsVerified;
        user.Role = role;

        await _userRepository.UpdateAsync(user);
        return ApiResponse<UserResponse>.SuccessResult(MapToResponse(user), "User updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        if (!await _userRepository.ExistsAsync(id))
            return ApiResponse<bool>.FailResult("User not found.");

        await _userRepository.DeleteAsync(id);
        return ApiResponse<bool>.SuccessResult(true, "User deleted successfully.");
    }

    // ── Mapping helpers ──────────────────────────────────────────────────────

    private static UserResponse MapToResponse(User user, decimal totalDeposited = 0m, decimal balance = 0m) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        AvatarUrl = user.AvatarUrl,
        Bio = user.Bio,
        Role = user.Role.ToString(),
        IsVerified = user.IsVerified,
        StudentVerificationStatus = user.StudentVerificationStatus ?? "NONE",
        StudentCardPath = user.StudentCardPath,
        HasSeenStudentRejection = user.HasSeenStudentRejection,
        TotalDeposited = totalDeposited,
        Balance = balance,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };

    private static UserDetailResponse MapToDetailResponse(User user, decimal totalDeposited = 0m, decimal balance = 0m) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        AvatarUrl = user.AvatarUrl,
        Bio = user.Bio,
        Role = user.Role.ToString(),
        IsVerified = user.IsVerified,
        StudentVerificationStatus = user.StudentVerificationStatus ?? "NONE",
        StudentCardPath = user.StudentCardPath,
        HasSeenStudentRejection = user.HasSeenStudentRejection,
        TotalDeposited = totalDeposited,
        Balance = balance,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        Enrollments = user.Enrollments.Select(e => new EnrollmentSummaryResponse
        {
            CourseId = e.CourseId,
            CourseTitle = e.Course?.Title ?? string.Empty,
            CourseSlug = e.Course?.Slug ?? string.Empty,
            ProgressPercentage = e.ProgressPercentage,
            EnrolledAt = e.EnrolledAt,
            CompletedAt = e.CompletedAt,
            LastAccessedAt = e.LastAccessedAt
        }).ToList()
    };

    
}
