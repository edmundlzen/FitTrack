using FitTrack.Data;
using FitTrack.Models;
using FitTrack.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FitTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Landing()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index");
            return View();
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var vm = new DashboardViewModel();

            if (userId != null)
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
                vm.WelcomeName = profile?.FullName ?? User.Identity?.Name ?? "Athlete";

                vm.TotalWorkouts = await _context.WorkoutSessions.CountAsync(w => w.UserId == userId);
                vm.TotalCaloriesBurned = await _context.WorkoutSessions
                    .Where(w => w.UserId == userId)
                    .SumAsync(w => (int?)w.CaloriesBurned) ?? 0;

                var today = DateTime.Today;
                vm.MealsLoggedToday = await _context.MealLogs
                    .CountAsync(m => m.UserId == userId && m.LogDate.Date == today);
                vm.ActiveGoals = await _context.FitnessGoals
                    .CountAsync(g => g.UserId == userId && g.Status == "In Progress");

                // Last 7 days chart data
                for (int i = 6; i >= 0; i--)
                {
                    var day = today.AddDays(-i);
                    vm.ChartLabels.Add(day.ToString("ddd MM/dd"));
                    vm.ChartData.Add(await _context.WorkoutSessions
                        .CountAsync(w => w.UserId == userId && w.SessionDate.Date == day));
                }
            }
            else
            {
                // Show seeded data totals for unauthenticated view
                vm.WelcomeName = "Athlete";
                var today = DateTime.Today;
                for (int i = 6; i >= 0; i--)
                {
                    vm.ChartLabels.Add(today.AddDays(-i).ToString("ddd MM/dd"));
                    vm.ChartData.Add(0);
                }
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
