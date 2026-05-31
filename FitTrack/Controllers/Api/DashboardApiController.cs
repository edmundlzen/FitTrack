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
    [Route("api/dashboard")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = _userManager.GetUserId(User)!;
            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);

            var todayMeals = await _context.MealLogs
                .Where(m => m.UserId == userId && m.LogDate.Date == today)
                .ToListAsync();

            var weekWorkouts = await _context.WorkoutSessions
                .Where(w => w.UserId == userId && w.SessionDate.Date >= weekAgo)
                .ToListAsync();

            var activeGoals = await _context.FitnessGoals
                .CountAsync(g => g.UserId == userId && g.Status == "In Progress");

            var recentMeals = await _context.MealLogs
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.LogDate)
                .Take(5)
                .ToListAsync();

            var recentWorkouts = await _context.WorkoutSessions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.SessionDate)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                TodayCalories = todayMeals.Sum(m => m.Calories),
                TodayProtein = todayMeals.Sum(m => m.Protein),
                TodayCarbs = todayMeals.Sum(m => m.Carbohydrates),
                TodayFats = todayMeals.Sum(m => m.Fats),
                WeekWorkouts = weekWorkouts.Count,
                WeekCaloriesBurned = weekWorkouts.Sum(w => w.CaloriesBurned),
                ActiveGoals = activeGoals,
                RecentMeals = recentMeals,
                RecentWorkouts = recentWorkouts
            });
        }
    }
}
