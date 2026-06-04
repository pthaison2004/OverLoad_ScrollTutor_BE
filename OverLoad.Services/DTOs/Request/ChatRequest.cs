using System.ComponentModel.DataAnnotations;

namespace OverLoad.Services.DTOs.Request;

public class ChatRequest
{
    [Required, MinLength(1), MaxLength(4000)]
    public string Message { get; set; } = string.Empty;
    public List<ChatHistoryItem> RecentHistory { get; set; } = new();
}

public class ChatHistoryItem
{
    [Required]
    public string Role { get; set; } = string.Empty;  // "user" | "model"

    [Required, MaxLength(4000)]
    public string Content { get; set; } = string.Empty;
}