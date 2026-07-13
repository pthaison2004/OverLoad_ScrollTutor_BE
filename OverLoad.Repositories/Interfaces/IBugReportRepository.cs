using OverLoad.Domain.Entities;
using OverLoad.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OverLoad.Repositories.Interfaces;

public interface IBugReportRepository : IBaseRepository<BugReport>
{
    Task<BugReport?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<BugReport>> GetByCourseIdWithDetailsAsync(int courseId);
    Task<IEnumerable<BugReport>> GetByUserIdWithDetailsAsync(int userId);
    Task<(IEnumerable<BugReport> Items, int TotalCount)> SearchAsync(
        int page, int pageSize, int? courseId, BugReportStatus? status, string? category, string? searchTerm);
}
