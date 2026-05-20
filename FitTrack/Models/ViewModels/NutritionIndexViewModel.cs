namespace FitTrack.Models.ViewModels
{
    public class NutritionIndexViewModel
    {
        public List<MealLog> AllMeals { get; set; } = new();
        public Dictionary<string, List<MealLog>> MealsByType { get; set; } = new();
        public int TodayCalories { get; set; }
        public double TodayProtein { get; set; }
        public double TodayCarbohydrates { get; set; }
        public double TodayFats { get; set; }
    }
}
