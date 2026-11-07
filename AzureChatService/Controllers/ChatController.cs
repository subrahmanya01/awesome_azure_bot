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
        public IActionResult Get(object recieved)
        {
            return Ok($"Chat service is running. {JsonSerializer.Serialize(recieved)}");
        }
    }
}
