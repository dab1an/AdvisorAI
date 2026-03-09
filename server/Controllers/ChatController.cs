using Microsoft.AspNetCore.Mvc;
using server.Services;

namespace server.Controllers;

[Route("api/Chat")]
[ApiController]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Chat(
        [FromForm] string message,
        IFormFile? file = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            return BadRequest("Message cannot be empty.");

        string response = await chatService.GetResponseAsync(message, file);
        return Ok(new { response }); // we should always serialize as an object, not raw JSON string
    }
}
