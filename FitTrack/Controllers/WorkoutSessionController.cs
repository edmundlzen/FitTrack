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
    public class WorkoutSessionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkoutSessionController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? category)
        {
            var userId = _userManager.GetUserId(User)!;
            var weekStart = DateTime.Today.AddDays(-7);

            var allSessions = await _context.WorkoutSessions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.SessionDate)
                .ToListAsync();

            var weekSessions = allSessions.Where(w => w.SessionDate.Date >= weekStart).ToList();

            var filtered = string.IsNullOrEmpty(category)
                ? allSessions
                : allSessions.Where(w => w.Category == category).ToList();

            var vm = new WorkoutIndexViewModel
            {
                Sessions = filtered,
                SelectedCategory = category,
                WeekTotalSessions = weekSessions.Count,
                WeekTotalCalories = weekSessions.Sum(w => w.CaloriesBurned)
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new WorkoutSession { SessionDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutSession session)
        {
            session.UserId = _userManager.GetUserId(User)!;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return View(session);

            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var session = await _context.WorkoutSessions.FirstOrDefaultAsync(w => w.WorkoutSessionId == id && w.UserId == userId);
            if (session == null) return NotFound();
            return View(session);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutSession session)
        {
            var userId = _userManager.GetUserId(User)!;
            if (id != session.WorkoutSessionId) return BadRequest();

            session.UserId = userId;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return View(session);

            var existing = await _context.WorkoutSessions.FirstOrDefaultAsync(w => w.WorkoutSessionId == id && w.UserId == userId);
            if (existing == null) return NotFound();

            existing.WorkoutName = session.WorkoutName;
            existing.Category = session.Category;
            existing.DurationMinutes = session.DurationMinutes;
            existing.CaloriesBurned = session.CaloriesBurned;
            existing.SessionDate = session.SessionDate;
            existing.Notes = session.Notes;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var session = await _context.WorkoutSessions.FirstOrDefaultAsync(w => w.WorkoutSessionId == id && w.UserId == userId);
            if (session == null) return NotFound();
            return View(session);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var session = await _context.WorkoutSessions.FirstOrDefaultAsync(w => w.WorkoutSessionId == id && w.UserId == userId);
            if (session == null) return NotFound();
            return View(session);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var session = await _context.WorkoutSessions.FirstOrDefaultAsync(w => w.WorkoutSessionId == id && w.UserId == userId);
            if (session != null)
            {
                _context.WorkoutSessions.Remove(session);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
