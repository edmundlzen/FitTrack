using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models
{
    public class MealLog
    {
        public int MealLogId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, Display(Name = "Meal Name")]
        public string MealName { get; set; } = string.Empty;

        [Required, Display(Name = "Meal Type")]
        public string MealType { get; set; } = string.Empty;

        [Required, Range(0, 5000)]
        public int Calories { get; set; }

        [Required, Range(0, 500), Display(Name = "Protein (g)")]
        public double Protein { get; set; }

        [Required, Range(0, 1000), Display(Name = "Carbohydrates (g)")]
        public double Carbohydrates { get; set; }

        [Required, Range(0, 500), Display(Name = "Fats (g)")]
        public double Fats { get; set; }

        [Required, Display(Name = "Log Date"), DataType(DataType.Date)]
        public DateTime LogDate { get; set; } = DateTime.Today;

        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }
    }
}
