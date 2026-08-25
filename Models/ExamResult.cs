using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagementSystem.Models
{
    public class ExamResult
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        [Required]
        [StringLength(20)]
        public string ExamType { get; set; } = string.Empty; 

        public double MarksObtained { get; set; }

        public double TotalMarks { get; set; }

        [StringLength(5)]
        public string? Grade { get; set; }

        public int Semester { get; set; }

        public DateTime ExamDate { get; set; }
    }
}
