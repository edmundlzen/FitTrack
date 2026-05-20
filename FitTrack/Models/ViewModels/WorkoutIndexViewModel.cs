namespace FitTrack.Models.ViewModels
{
    public class WorkoutIndexViewModel
    {
        public List<WorkoutSession> Sessions { get; set; } = new();
        public string? SelectedCategory { get; set; }
        public int WeekTotalSessions { get; set; }
        public int WeekTotalCalories { get; set; }
        public List<string> Categories { get; set; } = new() { "Cardio", "Strength", "Flexibility" };
    }
}
