using System.ComponentModel.DataAnnotations;

namespace CollegeManagementSystem.Models
{
    public class Notice
    {
        [Key]
        public int NoticeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(100)]
        public string? PostedBy { get; set; }

        [Required]
        [StringLength(20)]
        public string TargetRole { get; set; } = "All"; 

        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [StringLength(20)]
        public string Priority { get; set; } = "Normal"; 
    }
}
