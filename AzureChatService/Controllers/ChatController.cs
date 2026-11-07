using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AzureChatService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        public ChatController()
        {
            
        }

        [HttpPost("echo")]
        public IActionResult PostEcho(object recieved)
        {
            return Ok($"Chat service is running. {JsonSerializer.Serialize(recieved)}");
        }

        [HttpGet("echo")]
        public IActionResult GetEcho()
        {
            return Ok($"Hey your app is running");
        }
    }
}
