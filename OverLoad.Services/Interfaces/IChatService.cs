using OverLoad.Services.Common;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.DTOs.Response;

namespace OverLoad.Services.Interfaces;

public interface IChatService
{
    Task<ApiResponse<ChatResponse>> SendMessageAsync(int userId, ChatRequest request);
}