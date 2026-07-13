using Microsoft.EntityFrameworkCore;
using OverLoad.Domain.Entities;
using OverLoad.Domain.Enums;
using OverLoad.Repositories.Data;
using OverLoad.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverLoad.Repositories.Implementations;

public class BugReportRepository : BaseRepository<BugReport>, IBugReportRepository
{
    public BugReportRepository(AppDbContext context) : base(context) { }

    public async Task<BugReport?> GetByIdWithDetailsAsync(int id)
        => await _dbSet
            .Include(r => r.User)
            .Include(r => r.Course)
            .Include(r => r.Lesson)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<BugReport>> GetByCourseIdWithDetailsAsync(int courseId)
        => await _dbSet
            .Include(r => r.User)
            .Include(r => r.Course)
            .Include(r => r.Lesson)
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<BugReport>> GetByUserIdWithDetailsAsync(int userId)
        => await _dbSet
            .Include(r => r.User)
            .Include(r => r.Course)
            .Include(r => r.Lesson)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<(IEnumerable<BugReport> Items, int TotalCount)> SearchAsync(
        int page, int pageSize, int? courseId, BugReportStatus? status, string? category, string? searchTerm)
    {
        var query = _dbSet
            .Include(r => r.User)
            .Include(r => r.Course)
            .Include(r => r.Lesson)
            .AsQueryable();

        if (courseId.HasValue)
            query = query.Where(r => r.CourseId == courseId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(r => r.Course.Category == category);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(r =>
                r.Title.Contains(searchTerm) ||
                r.Description.Contains(searchTerm) ||
                r.User.FullName.Contains(searchTerm) ||
                r.Course.Title.Contains(searchTerm) ||
                (r.Lesson != null && r.Lesson.Title.Contains(searchTerm)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
