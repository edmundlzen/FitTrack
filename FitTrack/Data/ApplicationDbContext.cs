using FitTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<MealLog> MealLogs { get; set; }
        public DbSet<FitnessGoal> FitnessGoals { get; set; }
    }
}
