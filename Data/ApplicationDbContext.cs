using CollegeManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<ExamResult> ExamResults { get; set; } = null!;
        public DbSet<FeeRecord> FeeRecords { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public DbSet<Notice> Notices { get; set; } = null!;
        public DbSet<Timetable> Timetables { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Student>()
                .HasIndex(s => s.RollNumber)
                .IsUnique();

            
            builder.Entity<Teacher>()
                .HasIndex(t => t.EmployeeId)
                .IsUnique();

            
            builder.Entity<Course>()
                .HasIndex(c => c.CourseCode)
                .IsUnique();

            
            builder.Entity<Attendance>()
                .HasIndex(a => new { a.StudentId, a.CourseId, a.Date })
                .IsUnique();

            
            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.MarkedBy)
                .WithMany()
                .HasForeignKey(a => a.MarkedByTeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            
            builder.Entity<ExamResult>()
                .HasOne(r => r.Student)
                .WithMany(s => s.Results)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamResult>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Results)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<FeeRecord>()
                .HasOne(f => f.Student)
                .WithMany(s => s.FeeRecords)
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<LeaveRequest>()
                .HasOne(l => l.Student)
                .WithMany(s => s.LeaveRequests)
                .HasForeignKey(l => l.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            
            builder.Entity<Timetable>()
                .HasOne(t => t.Course)
                .WithMany(c => c.Timetables)
                .HasForeignKey(t => t.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
