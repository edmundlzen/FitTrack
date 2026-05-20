using FitTrack.Data;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public UserProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
                return RedirectToAction(nameof(Create));
            return RedirectToAction(nameof(Details), new { id = profile.UserProfileId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserProfileId == id && p.UserId == userId);
            if (profile == null) return NotFound();
            return View(profile);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User)!;
            var existing = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (existing != null)
                return RedirectToAction(nameof(Details), new { id = existing.UserProfileId });
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfile profile, IFormFile? profilePicture)
        {
            var userId = _userManager.GetUserId(User)!;
            profile.UserId = userId;
            profile.CreatedAt = DateTime.Now;

            ModelState.Remove("UserId");
            if (profilePicture != null)
                profile.ProfilePicture = await SaveProfilePicture(profilePicture);

            if (!ModelState.IsValid) return View(profile);

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = profile.UserProfileId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserProfileId == id && p.UserId == userId);
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserProfile profile, IFormFile? profilePicture)
        {
            var userId = _userManager.GetUserId(User)!;
            if (id != profile.UserProfileId) return BadRequest();

            var existing = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserProfileId == id && p.UserId == userId);
            if (existing == null) return NotFound();

            existing.FullName = profile.FullName;
            existing.Age = profile.Age;
            existing.Gender = profile.Gender;
            existing.Height = profile.Height;
            existing.Weight = profile.Weight;

            if (profilePicture != null)
                existing.ProfilePicture = await SaveProfilePicture(profilePicture);

            if (!ModelState.IsValid) return View(profile);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserProfileId == id && p.UserId == userId);
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserProfileId == id && p.UserId == userId);
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Create));
        }

        private async Task<string> SaveProfilePicture(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }
    }
}
