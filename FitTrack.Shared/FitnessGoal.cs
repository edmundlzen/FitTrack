using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models
{
    public class FitnessGoal
    {
        public int FitnessGoalId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, Display(Name = "Goal Title")]
        public string GoalTitle { get; set; } = string.Empty;

        [Required, Display(Name = "Goal Type")]
        public string GoalType { get; set; } = string.Empty;

        [Required, Display(Name = "Target Value"), Range(0, 10000)]
        public double TargetValue { get; set; }

        [Required, Display(Name = "Current Value"), Range(0, 10000)]
        public double CurrentValue { get; set; }

        [Required, Display(Name = "Start Date"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required, Display(Name = "Target Date"), DataType(DataType.Date)]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddMonths(3);

        [Required]
        public string Status { get; set; } = "In Progress";

        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        public int ProgressPercentage => TargetValue > 0
            ? (int)Math.Min(100, CurrentValue / TargetValue * 100)
            : 0;

        public string StatusBadgeClass => Status switch
        {
            "Completed" => "bg-success",
            "Abandoned" => "bg-danger",
            _ => "bg-warning text-dark"
        };
    }
}
