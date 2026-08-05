using Microsoft.EntityFrameworkCore;
using OverLoad.Domain.Entities;
using OverLoad.Repositories.Data;
using OverLoad.Services.Interfaces;

namespace OverLoad.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;

    public SubscriptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SyncUserSubscriptionsAsync(int userId)
    {
        var subscriptionCourses = await _context.Courses
            .Where(c => c.Slug.Contains("pro-upgrade") || c.Slug.Contains("plus-upgrade"))
            .ToListAsync();

        if (!subscriptionCourses.Any()) return;

        var courseIds = subscriptionCourses.Select(c => c.Id).ToList();

        // Lấy tất cả giao dịch SUCCESS mua gói nâng cấp của user theo thứ tự thời gian
        var successTransactions = await _context.Transactions
            .Where(t => t.UserId == userId && t.Status == "SUCCESS" && courseIds.Contains(t.CourseId))
            .OrderBy(t => t.PaymentTime)
            .ToListAsync();

        if (!successTransactions.Any()) return;

        var groupedByCourse = successTransactions.GroupBy(t => t.CourseId);

        foreach (var group in groupedByCourse)
        {
            var course = subscriptionCourses.First(c => c.Id == group.Key);
            int durationDays = course.Slug.Contains("year") ? 365 : 30;

            DateTime effectiveStart = DateTime.MinValue;

            foreach (var t in group)
            {
                // Nếu lần mua trước đã hết hạn trước thời điểm mua này, tính từ ngày mua mới;
                // Nếu lần mua trước chưa hết hạn, nối tiếp từ thời điểm hết hạn của lần mua trước!
                if (effectiveStart == DateTime.MinValue || effectiveStart.AddDays(durationDays) < t.PaymentTime)
                {
                    effectiveStart = t.PaymentTime;
                }
                else
                {
                    effectiveStart = effectiveStart.AddDays(durationDays);
                }
            }

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == group.Key);

            if (enrollment == null)
            {
                _context.Enrollments.Add(new Enrollment
                {
                    UserId = userId,
                    CourseId = group.Key,
                    EnrolledAt = effectiveStart
                });
            }
            else
            {
                enrollment.EnrolledAt = effectiveStart;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<string> GetUserActivePlanAsync(int userId)
    {
        // Tự động đồng bộ gói dịch vụ từ giao dịch trước khi kiểm tra plan
        await SyncUserSubscriptionsAsync(userId);

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.UserId == userId &&
                       (e.Course.Slug.Contains("pro-upgrade") || e.Course.Slug.Contains("plus-upgrade")))
            .ToListAsync();

        bool hasActivePro = false;
        bool hasActivePlus = false;
        var now = DateTime.UtcNow;

        foreach (var e in enrollments)
        {
            int durationDays = e.Course.Slug.Contains("month") ? 30 : 365;
            var expirationDate = e.EnrolledAt.AddDays(durationDays);
            if (expirationDate > now)
            {
                if (e.Course.Slug.Contains("pro-upgrade"))
                    hasActivePro = true;
                else if (e.Course.Slug.Contains("plus-upgrade"))
                    hasActivePlus = true;
            }
        }

        if (hasActivePro) return "PRO";
        if (hasActivePlus) return "PLUS";
        return "FREE";
    }

    public async Task<int> GetRemainingAiQuestionsAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return 0;

        var activePlan = await GetUserActivePlanAsync(userId);

        int dailyLimit = activePlan switch
        {
            "PRO" => -1, // unlimited
            "PLUS" => 20,
            _ => 3
        };

        if (dailyLimit == -1) return -1;

        // Reset counter if last question was on a different UTC day
        var today = DateTime.UtcNow.Date;
        if (user.LastAiQuestionDate.ToUniversalTime().Date < today)
        {
            return dailyLimit;
        }

        return Math.Max(0, dailyLimit - user.AiQuestionsAskedToday);
    }
}
