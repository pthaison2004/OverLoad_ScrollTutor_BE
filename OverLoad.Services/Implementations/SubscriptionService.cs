using Microsoft.EntityFrameworkCore;
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

    public async Task<string> GetUserActivePlanAsync(int userId)
    {
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
