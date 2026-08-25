using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagementSystem.Models
{
    public class LeaveRequest
    {
        [Key]
        public int LeaveId { get; set; }

        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        public int? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher? TeacherNav { get; set; }

        [Required]
        [StringLength(30)]
        public string LeaveType { get; set; } = string.Empty; 

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Status { get; set; } = "Pending"; 

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    }
}
