using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagementSystem.Models
{
    public class FeeRecord
    {
        [Key]
        public int FeeId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [Required]
        [StringLength(30)]
        public string FeeType { get; set; } = string.Empty; 

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        [Required]
        [StringLength(15)]
        public string Status { get; set; } = "Pending"; 

        [StringLength(50)]
        public string? TransactionId { get; set; }

        public int Semester { get; set; }
    }
}
