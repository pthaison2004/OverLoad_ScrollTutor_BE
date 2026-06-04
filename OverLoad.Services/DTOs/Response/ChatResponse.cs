namespace OverLoad.Services.DTOs.Response;

public class ChatResponse
{
    public string Reply { get; set; } = string.Empty;
    public bool IsBlocked { get; set; } = false;
    public string? BlockReason { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
}