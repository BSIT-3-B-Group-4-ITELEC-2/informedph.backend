using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using informedph.backend.Data;
using informedph.backend.Models;

namespace informedph.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscribersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/subscribers/subscribe
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] Subscriber subscriber)
        {
            if (subscriber == null || string.IsNullOrEmpty(subscriber.email))
            {
                return BadRequest(new { message = "Invalid subscriber data." });
            }

            // Check if email already exists to prevent duplicates
            var existingSubscriber = await _context.Subscribers
                .FirstOrDefaultAsync(s => s.email == subscriber.email);

            if (existingSubscriber != null)
            {
                return Conflict(new { message = "This email is already subscribed." });
            }

            _context.Subscribers.Add(subscriber);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully subscribed!", subscriber });
        }

        // DELETE: api/subscribers/unsubscribe?email=test@example.com
        [HttpDelete("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email parameter is required." });
            }

            var subscriber = await _context.Subscribers
                .FirstOrDefaultAsync(s => s.email == email);

            if (subscriber == null)
            {
                return NotFound(new { message = "Subscriber email not found." });
            }

            _context.Subscribers.Remove(subscriber);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully unsubscribed." });
        }
    }
}