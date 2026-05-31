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
    [Route("api/profile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserProfileApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = _userManager.GetUserId(User)!;
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] UserProfile profile)
        {
            var userId = _userManager.GetUserId(User)!;
            profile.UserId = userId;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existing == null)
            {
                profile.CreatedAt = DateTime.Now;
                _context.UserProfiles.Add(profile);
            }
            else
            {
                existing.FullName = profile.FullName;
                existing.Age = profile.Age;
                existing.Gender = profile.Gender;
                existing.Height = profile.Height;
                existing.Weight = profile.Weight;
            }

            await _context.SaveChangesAsync();
            return Ok(existing ?? profile);
        }
    }
}
