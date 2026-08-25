using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagementSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [StringLength(20)]
        public string RollNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        public int Semester { get; set; }

        [StringLength(10)]
        public string? Section { get; set; }

        public DateTime DateOfBirth { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? GuardianName { get; set; }

        [StringLength(15)]
        public string? GuardianPhone { get; set; }

        
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
        public virtual ICollection<FeeRecord> FeeRecords { get; set; } = new List<FeeRecord>();
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
