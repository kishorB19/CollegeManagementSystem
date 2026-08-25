using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using CollegeManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFeeService _feeService;

        public DashboardService(ApplicationDbContext context, IFeeService feeService)
        {
            _context = context;
            _feeService = feeService;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalTeachers = await _context.Teachers.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalNotices = await _context.Notices.CountAsync(n => n.IsActive);

            var totalFeeCollected = await _feeService.GetTotalCollectedAsync();
            var totalFeePending = await _feeService.GetTotalPendingAsync();

            
            var totalAttendance = await _context.Attendances.CountAsync();
            var totalPresent = await _context.Attendances.CountAsync(a => a.Status == "Present" || a.Status == "Late");
            var overallAttendance = totalAttendance > 0 ? Math.Round((double)totalPresent / totalAttendance * 100, 1) : 0;

            var recentNotices = await _context.Notices
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.PostedDate)
                .Take(5)
                .ToListAsync();

            var recentStudents = await _context.Students
                .Include(s => s.User)
                .OrderByDescending(s => s.AdmissionDate)
                .Take(5)
                .ToListAsync();

            var deptStudents = await GetDepartmentWiseStudentsAsync();
            var monthlyAttendance = await GetMonthlyAttendanceAsync();

            return new AdminDashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalCourses = totalCourses,
                TotalNotices = totalNotices,
                TotalFeeCollected = totalFeeCollected,
                TotalFeePending = totalFeePending,
                OverallAttendancePercentage = overallAttendance,
                RecentNotices = recentNotices,
                RecentStudents = recentStudents,
                DepartmentWiseStudents = deptStudents,
                MonthlyAttendance = monthlyAttendance
            };
        }

        public async Task<Dictionary<string, int>> GetDepartmentWiseStudentsAsync()
        {
            
            return await _context.Students
                .GroupBy(s => s.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Department, x => x.Count);
        }

        public async Task<Dictionary<string, double>> GetMonthlyAttendanceAsync()
        {
            
            
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var attendances = await _context.Attendances
                .Where(a => a.Date >= sixMonthsAgo)
                .ToListAsync();

            return attendances
                .GroupBy(a => a.Date.ToString("MMM yyyy"))
                .OrderBy(g => g.Min(a => a.Date))
                .ToDictionary(
                    g => g.Key,
                    g => g.Count() > 0 ? Math.Round((double)g.Count(a => a.Status == "Present" || a.Status == "Late") / g.Count() * 100, 1) : 0
                );
        }
    }
}
