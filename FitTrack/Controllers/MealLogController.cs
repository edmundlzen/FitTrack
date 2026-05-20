using FitTrack.Data;
using FitTrack.Models;
using FitTrack.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers
{
    [Authorize]
    public class MealLogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MealLogController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var today = DateTime.Today;

            var allMeals = await _context.MealLogs
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.LogDate)
                .ToListAsync();

            var todayMeals = allMeals.Where(m => m.LogDate.Date == today).ToList();

            var vm = new NutritionIndexViewModel
            {
                AllMeals = allMeals,
                MealsByType = allMeals
                    .GroupBy(m => m.MealType)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                TodayCalories = todayMeals.Sum(m => m.Calories),
                TodayProtein = todayMeals.Sum(m => m.Protein),
                TodayCarbohydrates = todayMeals.Sum(m => m.Carbohydrates),
                TodayFats = todayMeals.Sum(m => m.Fats)
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new MealLog { LogDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealLog meal)
        {
            meal.UserId = _userManager.GetUserId(User)!;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return View(meal);

            _context.MealLogs.Add(meal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var meal = await _context.MealLogs.FirstOrDefaultAsync(m => m.MealLogId == id && m.UserId == userId);
            if (meal == null) return NotFound();
            return View(meal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MealLog meal)
        {
            var userId = _userManager.GetUserId(User)!;
            if (id != meal.MealLogId) return BadRequest();

            meal.UserId = userId;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return View(meal);

            var existing = await _context.MealLogs.FirstOrDefaultAsync(m => m.MealLogId == id && m.UserId == userId);
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
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var meal = await _context.MealLogs.FirstOrDefaultAsync(m => m.MealLogId == id && m.UserId == userId);
            if (meal == null) return NotFound();
            return View(meal);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var meal = await _context.MealLogs.FirstOrDefaultAsync(m => m.MealLogId == id && m.UserId == userId);
            if (meal == null) return NotFound();
            return View(meal);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var meal = await _context.MealLogs.FirstOrDefaultAsync(m => m.MealLogId == id && m.UserId == userId);
            if (meal != null)
            {
                _context.MealLogs.Remove(meal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
