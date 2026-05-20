using FitTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await context.Database.MigrateAsync();

            // Create demo user if none exist
            if (!await context.Users.AnyAsync())
            {
                var demoUser = new IdentityUser
                {
                    UserName = "demo@fittrack.com",
                    Email = "demo@fittrack.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(demoUser, "Demo@123!");

                var userId = demoUser.Id;
                var today = DateTime.Today;

                // Seed UserProfile
                context.UserProfiles.Add(new UserProfile
                {
                    UserId = userId,
                    FullName = "Alex Johnson",
                    Age = 28,
                    Gender = "Male",
                    Height = 178,
                    Weight = 80,
                    CreatedAt = today.AddMonths(-2)
                });

                // Seed WorkoutSessions (last 10 days)
                var workouts = new List<WorkoutSession>
                {
                    new() { UserId = userId, WorkoutName = "Morning Run", Category = "Cardio", DurationMinutes = 35, CaloriesBurned = 320, SessionDate = today, Notes = "5km at steady pace" },
                    new() { UserId = userId, WorkoutName = "Bench Press & Chest", Category = "Strength", DurationMinutes = 50, CaloriesBurned = 280, SessionDate = today.AddDays(-1) },
                    new() { UserId = userId, WorkoutName = "Yoga Flow", Category = "Flexibility", DurationMinutes = 45, CaloriesBurned = 150, SessionDate = today.AddDays(-2), Notes = "Hatha yoga session" },
                    new() { UserId = userId, WorkoutName = "HIIT Intervals", Category = "Cardio", DurationMinutes = 30, CaloriesBurned = 400, SessionDate = today.AddDays(-2) },
                    new() { UserId = userId, WorkoutName = "Deadlifts & Back", Category = "Strength", DurationMinutes = 60, CaloriesBurned = 350, SessionDate = today.AddDays(-3) },
                    new() { UserId = userId, WorkoutName = "Cycling", Category = "Cardio", DurationMinutes = 45, CaloriesBurned = 380, SessionDate = today.AddDays(-4), Notes = "Outdoor ride" },
                    new() { UserId = userId, WorkoutName = "Leg Day", Category = "Strength", DurationMinutes = 55, CaloriesBurned = 310, SessionDate = today.AddDays(-5) },
                    new() { UserId = userId, WorkoutName = "Pilates", Category = "Flexibility", DurationMinutes = 40, CaloriesBurned = 180, SessionDate = today.AddDays(-5) },
                    new() { UserId = userId, WorkoutName = "Swimming Laps", Category = "Cardio", DurationMinutes = 50, CaloriesBurned = 430, SessionDate = today.AddDays(-6) },
                    new() { UserId = userId, WorkoutName = "Shoulder Press", Category = "Strength", DurationMinutes = 45, CaloriesBurned = 260, SessionDate = today.AddDays(-7) },
                };
                context.WorkoutSessions.AddRange(workouts);

                // Seed MealLogs
                var meals = new List<MealLog>
                {
                    new() { UserId = userId, MealName = "Oatmeal with Berries", MealType = "Breakfast", Calories = 380, Protein = 12, Carbohydrates = 65, Fats = 8, LogDate = today },
                    new() { UserId = userId, MealName = "Grilled Chicken Salad", MealType = "Lunch", Calories = 520, Protein = 45, Carbohydrates = 30, Fats = 18, LogDate = today },
                    new() { UserId = userId, MealName = "Protein Bar", MealType = "Snack", Calories = 210, Protein = 20, Carbohydrates = 25, Fats = 6, LogDate = today },
                    new() { UserId = userId, MealName = "Salmon & Vegetables", MealType = "Dinner", Calories = 650, Protein = 50, Carbohydrates = 40, Fats = 22, LogDate = today },
                    new() { UserId = userId, MealName = "Greek Yogurt Parfait", MealType = "Breakfast", Calories = 320, Protein = 18, Carbohydrates = 42, Fats = 7, LogDate = today.AddDays(-1) },
                    new() { UserId = userId, MealName = "Turkey Wrap", MealType = "Lunch", Calories = 480, Protein = 38, Carbohydrates = 45, Fats = 14, LogDate = today.AddDays(-1) },
                    new() { UserId = userId, MealName = "Almonds", MealType = "Snack", Calories = 160, Protein = 6, Carbohydrates = 6, Fats = 14, LogDate = today.AddDays(-1) },
                    new() { UserId = userId, MealName = "Beef Stir Fry", MealType = "Dinner", Calories = 720, Protein = 48, Carbohydrates = 55, Fats = 25, LogDate = today.AddDays(-1) },
                };
                context.MealLogs.AddRange(meals);

                // Seed FitnessGoals
                var goals = new List<FitnessGoal>
                {
                    new() { UserId = userId, GoalTitle = "Lose 5kg", GoalType = "Weight Loss", TargetValue = 75, CurrentValue = 80, StartDate = today.AddMonths(-2), TargetDate = today.AddMonths(2), Status = "In Progress", Notes = "Targeting 0.5kg/week reduction" },
                    new() { UserId = userId, GoalTitle = "Run 10km", GoalType = "Endurance", TargetValue = 10, CurrentValue = 6.5, StartDate = today.AddMonths(-1), TargetDate = today.AddMonths(2), Status = "In Progress", Notes = "Building up distance gradually" },
                    new() { UserId = userId, GoalTitle = "Bench Press 100kg", GoalType = "Muscle Gain", TargetValue = 100, CurrentValue = 100, StartDate = today.AddMonths(-4), TargetDate = today.AddDays(-7), Status = "Completed", Notes = "Hit the goal!" },
                    new() { UserId = userId, GoalTitle = "Full Split", GoalType = "Flexibility", TargetValue = 100, CurrentValue = 40, StartDate = today.AddMonths(-1), TargetDate = today.AddMonths(5), Status = "In Progress" },
                };
                context.FitnessGoals.AddRange(goals);

                await context.SaveChangesAsync();
            }
        }
    }
}
