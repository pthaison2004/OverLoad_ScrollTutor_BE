using OverLoad.Services.Common;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OverLoad.Services.Interfaces;

public interface IBugReportService
{
    Task<ApiResponse<BugReportResponse>> CreateAsync(int userId, CreateBugReportRequest request);
    Task<ApiResponse<BugReportResponse>> UpdateStatusAsync(int id, UpdateBugReportStatusRequest request);
    Task<ApiResponse<List<BugReportResponse>>> GetByCourseIdAsync(int courseId);
    Task<ApiResponse<List<BugReportResponse>>> GetByUserIdAsync(int userId);
    Task<PagedResponse<BugReportResponse>> SearchAsync(BugReportQueryParams queryParams);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
