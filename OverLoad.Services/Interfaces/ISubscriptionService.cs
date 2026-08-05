using OverLoad.Services.Common;

namespace OverLoad.Services.Interfaces;

public interface ISubscriptionService
{
    /// <summary>
    /// Xác định gói cước đang hoạt động của user: "FREE", "PLUS", hoặc "PRO".
    /// </summary>
    Task<string> GetUserActivePlanAsync(int userId);

    /// <summary>
    /// Đồng bộ lại Enrollment cho tất cả gói cước đã mua thành công của user.
    /// </summary>
    Task SyncUserSubscriptionsAsync(int userId);

    /// <summary>
    /// Kiểm tra và trả về số lượt hỏi AI còn lại trong ngày, hoặc -1 nếu không giới hạn (PRO).
    /// </summary>
    Task<int> GetRemainingAiQuestionsAsync(int userId);
}
