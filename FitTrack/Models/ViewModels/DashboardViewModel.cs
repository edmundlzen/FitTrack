namespace FitTrack.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string WelcomeName { get; set; } = "Athlete";
        public int TotalWorkouts { get; set; }
        public int TotalCaloriesBurned { get; set; }
        public int MealsLoggedToday { get; set; }
        public int ActiveGoals { get; set; }
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartData { get; set; } = new();
    }
}
