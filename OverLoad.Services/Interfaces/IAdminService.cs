using OverLoad.Services.Common;
using OverLoad.Services.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLoad.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<AdminDashboardResponse>> GetDashboardAsync();
        Task<ApiResponse<bool>> ChangeRoleAsync(int userId, string role);
        Task<ApiResponse<bool>> ToggleLockAsync(int userId);
    }
}
