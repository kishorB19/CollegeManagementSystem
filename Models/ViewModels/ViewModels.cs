namespace CollegeManagementSystem.Models.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
    public class RegisterViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
        public string? Department { get; set; }
    }
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalNotices { get; set; }
        public decimal TotalFeeCollected { get; set; }
        public decimal TotalFeePending { get; set; }
        public double OverallAttendancePercentage { get; set; }
        public List<Notice> RecentNotices { get; set; } = new();
        public List<Student> RecentStudents { get; set; } = new();
        public Dictionary<string, int> DepartmentWiseStudents { get; set; } = new();
        public Dictionary<string, double> MonthlyAttendance { get; set; } = new();
    }
    public class TeacherDashboardViewModel
    {
        public Teacher? Teacher { get; set; }
        public List<Course> MyCourses { get; set; } = new();
        public int TotalStudents { get; set; }
        public int TodayClasses { get; set; }
        public int PendingLeaves { get; set; }
        public List<Notice> RecentNotices { get; set; } = new();
        public Dictionary<string, double> CourseAttendance { get; set; } = new();
    }
    public class StudentDashboardViewModel
    {
        public Student? Student { get; set; }
        public double OverallAttendance { get; set; }
        public double GPA { get; set; }
        public decimal TotalFeesPaid { get; set; }
        public decimal TotalFeesPending { get; set; }
        public int PendingLeaves { get; set; }
        public List<Notice> RecentNotices { get; set; } = new();
        public List<AttendanceSummary> CourseAttendance { get; set; } = new();
        public List<ExamResult> RecentResults { get; set; } = new();
        public List<FeeRecord> FeeRecords { get; set; } = new();
    }
    public class AttendanceSummary
    {
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int TotalClasses { get; set; }
        public int PresentClasses { get; set; }
        public double Percentage { get; set; }
    }
    public class MarkAttendanceViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public List<StudentAttendanceEntry> Students { get; set; } = new();
    }
    public class StudentAttendanceEntry
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Present";
    }
    public class AddResultViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public double TotalMarks { get; set; }
        public int Semester { get; set; }
        public DateTime ExamDate { get; set; } = DateTime.Today;
        public List<StudentResultEntry> Students { get; set; } = new();
    }
    public class StudentResultEntry
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public double MarksObtained { get; set; }
        public string? Grade { get; set; }
    }
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
