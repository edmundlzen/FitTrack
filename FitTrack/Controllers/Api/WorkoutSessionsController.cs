using FitTrack.Data;
using FitTrack.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers.Api
{
    [ApiController]
    [Route("api/workoutsessions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkoutSessionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User)!;
            var sessions = await _context.WorkoutSessions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.SessionDate)
                .ToListAsync();
            return Ok(sessions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkoutSession session)
        {
            session.UserId = _userManager.GetUserId(User)!;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = session.WorkoutSessionId }, session);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkoutSession session)
        {
            var userId = _userManager.GetUserId(User)!;
            if (id != session.WorkoutSessionId) return BadRequest();

            var existing = await _context.WorkoutSessions
                .FirstOrDefaultAsync(w => w.WorkoutSessionId == id && w.UserId == userId);
            if (existing == null) return NotFound();

            existing.WorkoutName = session.WorkoutName;
            existing.Category = session.Category;
            existing.DurationMinutes = session.DurationMinutes;
            existing.CaloriesBurned = session.CaloriesBurned;
            existing.SessionDate = session.SessionDate;
            existing.Notes = session.Notes;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var session = await _context.WorkoutSessions
                .FirstOrDefaultAsync(w => w.WorkoutSessionId == id && w.UserId == userId);
            if (session == null) return NotFound();

            _context.WorkoutSessions.Remove(session);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
