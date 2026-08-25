using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagementSystem.Models
{
    public class Timetable
    {
        [Key]
        public int TimetableId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        [Required]
        [StringLength(15)]
        public string Day { get; set; } = string.Empty; 

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(20)]
        public string? Room { get; set; }
    }
}
