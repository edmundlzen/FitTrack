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
    [Route("api/meallogs")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MealLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MealLogsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User)!;
            var meals = await _context.MealLogs
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.LogDate)
                .ToListAsync();
            return Ok(meals);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MealLog meal)
        {
            meal.UserId = _userManager.GetUserId(User)!;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.MealLogs.Add(meal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = meal.MealLogId }, meal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MealLog meal)
        {
            var userId = _userManager.GetUserId(User)!;
            if (id != meal.MealLogId) return BadRequest();

            var existing = await _context.MealLogs
                .FirstOrDefaultAsync(m => m.MealLogId == id && m.UserId == userId);
            if (existing == null) return NotFound();

            existing.MealName = meal.MealName;
            existing.MealType = meal.MealType;
            existing.Calories = meal.Calories;
            existing.Protein = meal.Protein;
            existing.Carbohydrates = meal.Carbohydrates;
            existing.Fats = meal.Fats;
            existing.LogDate = meal.LogDate;
            existing.Notes = meal.Notes;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var meal = await _context.MealLogs
                .FirstOrDefaultAsync(m => m.MealLogId == id && m.UserId == userId);
            if (meal == null) return NotFound();

            _context.MealLogs.Remove(meal);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
