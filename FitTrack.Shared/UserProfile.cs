using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, Range(1, 120)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required, Display(Name = "Height (cm)"), Range(50, 300)]
        public double Height { get; set; }

        [Required, Display(Name = "Weight (kg)"), Range(1, 500)]
        public double Weight { get; set; }

        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Member Since")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public double BMI => Height > 0 ? Math.Round(Weight / Math.Pow(Height / 100.0, 2), 1) : 0;

        public string BMICategory => BMI switch
        {
            < 18.5 => "Underweight",
            < 25.0 => "Normal",
            < 30.0 => "Overweight",
            _ => "Obese"
        };

        public string BMIBadgeClass => BMICategory switch
        {
            "Underweight" => "bg-info",
            "Normal" => "bg-success",
            "Overweight" => "bg-warning text-dark",
            _ => "bg-danger"
        };
    }
}
