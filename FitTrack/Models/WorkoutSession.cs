using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models
{
    public class WorkoutSession
    {
        public int WorkoutSessionId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, Display(Name = "Workout Name")]
        public string WorkoutName { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required, Display(Name = "Duration (minutes)"), Range(1, 600)]
        public int DurationMinutes { get; set; }

        [Required, Display(Name = "Calories Burned"), Range(0, 5000)]
        public int CaloriesBurned { get; set; }

        [Required, Display(Name = "Session Date"), DataType(DataType.Date)]
        public DateTime SessionDate { get; set; } = DateTime.Today;

        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }
    }
}
