using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagementSystem.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Designation { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Qualification { get; set; }

        
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
