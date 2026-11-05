using informedph.backend.Data;
using informedph.backend.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace informedph.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedule()
        {
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == 1);

            if (schedule == null)
                return NotFound("Schedule configuration not found.");

            return Ok(schedule);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSchedule(
            UpdateScheduleRequest request)
        {
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == 1);

            if (schedule == null)
                return NotFound("Schedule configuration not found.");

            schedule.ExecutionTime = request.ExecutionTime;
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.LastExecutedAt = null; // Added for demo purposes. To enable changing time executes the scheduler again.

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Schedule updated successfully.",
                executionTime = schedule.ExecutionTime,
                updatedAt = schedule.UpdatedAt
            });
        }
    }
}
