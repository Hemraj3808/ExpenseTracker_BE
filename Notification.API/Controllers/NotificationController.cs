using Microsoft.AspNetCore.Mvc;
using Notification.API.Models;
using Notification.API.Services;

namespace Notification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly EmailService _emailServices;

        public NotificationController(EmailService emailServices)
        {
            _emailServices = emailServices;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            await _emailServices.SendEmailAsync(request);
            return Ok("Email sent successfully.");
        }
    }
}
