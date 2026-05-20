using FitTrack.Data;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers
{
    [Authorize]
    public class FitnessGoalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FitnessGoalController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var goals = await _context.FitnessGoals
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.Status)
                .ThenBy(g => g.TargetDate)
                .ToListAsync();
            return View(goals);
        }

        public IActionResult Create()
        {
            return View(new FitnessGoal
            {
                StartDate = DateTime.Today,
                TargetDate = DateTime.Today.AddMonths(3),
                Status = "In Progress"
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FitnessGoal goal)
        {
            goal.UserId = _userManager.GetUserId(User)!;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return View(goal);

            _context.FitnessGoals.Add(goal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _context.FitnessGoals.FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
            if (goal == null) return NotFound();
            return View(goal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FitnessGoal goal)
        {
            var userId = _userManager.GetUserId(User)!;
            if (id != goal.FitnessGoalId) return BadRequest();

            goal.UserId = userId;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return View(goal);

            var existing = await _context.FitnessGoals.FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
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
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _context.FitnessGoals.FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
            if (goal == null) return NotFound();
            return View(goal);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _context.FitnessGoals.FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
            if (goal == null) return NotFound();
            return View(goal);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _context.FitnessGoals.FirstOrDefaultAsync(g => g.FitnessGoalId == id && g.UserId == userId);
            if (goal != null)
            {
                _context.FitnessGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
