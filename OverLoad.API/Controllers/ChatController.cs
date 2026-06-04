using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverLoad.Services.DTOs.Request;
using OverLoad.Services.Interfaces;

namespace OverLoad.API.Controllers;

/// <summary>AI coding assistant powered by Gemini.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
        => _chatService = chatService;

    /// <summary>
    /// Send a message to the AI coding assistant.
    /// Attach up to 3 recent exchanges in RecentHistory to maintain context.
    /// Server does NOT store any chat history.
    /// </summary>
    /// <remarks>
    /// Example request:
    ///
    ///     POST /api/chat
    ///     {
    ///       "message": "Explain what async/await does in C#",
    ///       "recentHistory": [
    ///         { "role": "user",  "content": "What is a delegate in C#?" },
    ///         { "role": "model", "content": "A delegate is a type-safe function pointer..." }
    ///       ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _chatService.SendMessageAsync(request);
        if (!result.Success) return StatusCode(500, result);
        return Ok(result);
    }
}