using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using CollegeManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAttendanceService _attendanceService;
        private readonly IResultService _resultService;

        public StudentService(ApplicationDbContext context, IAttendanceService attendanceService, IResultService resultService)
        {
            _context = context;
            _attendanceService = attendanceService;
            _resultService = resultService;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.User)
                .OrderBy(s => s.RollNumber)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student?> GetStudentByUserIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<StudentDashboardViewModel> GetStudentDashboardAsync(int studentId)
        {
            var student = await GetStudentByIdAsync(studentId);
            if (student == null) throw new ArgumentException("Student not found.", nameof(studentId));

            var overallAttendance = await _attendanceService.GetAttendancePercentageAsync(studentId);
            var gpa = await _resultService.CalculateGPAAsync(studentId);
            var courseAttendance = await _attendanceService.GetAttendanceSummaryAsync(studentId);
            var recentResults = await _resultService.GetResultsByStudentAsync(studentId);
            var feeRecords = await _context.FeeRecords
                .Where(f => f.StudentId == studentId)
                .OrderByDescending(f => f.DueDate)
                .ToListAsync();

            var pendingLeaves = await _context.LeaveRequests
                .CountAsync(l => l.StudentId == studentId && l.Status == "Pending");

            var notices = await _context.Notices
                .Where(n => n.IsActive && (n.TargetRole == "All" || n.TargetRole == "Students"))
                .OrderByDescending(n => n.PostedDate)
                .Take(5)
                .ToListAsync();

            return new StudentDashboardViewModel
            {
                Student = student,
                OverallAttendance = overallAttendance,
                GPA = gpa,
                TotalFeesPaid = feeRecords.Where(f => f.Status == "Paid").Sum(f => f.Amount),
                TotalFeesPending = feeRecords.Where(f => f.Status != "Paid").Sum(f => f.Amount),
                PendingLeaves = pendingLeaves,
                RecentNotices = notices,
                CourseAttendance = courseAttendance,
                RecentResults = recentResults,
                FeeRecords = feeRecords
            };
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Student>> GetStudentsByDepartmentAsync(string department)
        {
            return await _context.Students
                .Include(s => s.User)
                .Where(s => s.Department == department)
                .OrderBy(s => s.RollNumber)
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsBySemesterAsync(int semester)
        {
            return await _context.Students
                .Include(s => s.User)
                .Where(s => s.Semester == semester)
                .OrderBy(s => s.RollNumber)
                .ToListAsync();
        }
    }
}
