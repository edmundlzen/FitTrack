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
    [Route("api/fitnessgoals")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FitnessGoalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FitnessGoalsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User)!;
            var goals = await _context.FitnessGoals
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.Status)
                .ThenBy(g => g.TargetDate)
                .ToListAsync();
            return Ok(goals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _context.FitnessGoals
                .FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
            if (goal == null) return NotFound();
            return Ok(goal);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FitnessGoal goal)
        {
            goal.UserId = _userManager.GetUserId(User)!;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.FitnessGoals.Add(goal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = goal.FitnessGoalId }, goal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FitnessGoal goal)
        {
            var userId = _userManager.GetUserId(User)!;
            if (id != goal.FitnessGoalId) return BadRequest();

            var existing = await _context.FitnessGoals
                .FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
            if (existing == null) return NotFound();

            existing.GoalTitle = goal.GoalTitle;
            existing.GoalType = goal.GoalType;
            existing.TargetValue = goal.TargetValue;
            existing.CurrentValue = goal.CurrentValue;
            existing.StartDate = goal.StartDate;
            existing.TargetDate = goal.TargetDate;
            existing.Status = goal.Status;
            existing.Notes = goal.Notes;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _context.FitnessGoals
                .FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
            if (goal == null) return NotFound();

            _context.FitnessGoals.Remove(goal);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
