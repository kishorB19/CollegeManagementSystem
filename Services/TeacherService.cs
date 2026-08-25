using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using CollegeManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Courses)
                .OrderBy(t => t.EmployeeId)
                .ToListAsync();
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.TeacherId == id);
        }

        public async Task<Teacher?> GetTeacherByUserIdAsync(string userId)
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(int teacherId)
        {
            var teacher = await GetTeacherByIdAsync(teacherId);
            if (teacher == null) throw new ArgumentException("Teacher not found.", nameof(teacherId));

            var myCourses = await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();

            var courseIds = myCourses.Select(c => c.CourseId).ToList();

            
            var totalStudents = await _context.Attendances
                .Where(a => courseIds.Contains(a.CourseId))
                .Select(a => a.StudentId)
                .Distinct()
                .CountAsync();

            
            var today = DateTime.UtcNow.DayOfWeek.ToString();
            var todayClasses = await _context.Timetables
                .CountAsync(t => courseIds.Contains(t.CourseId) && t.Day == today);

            var pendingLeaves = await _context.LeaveRequests
                .CountAsync(l => l.Status == "Pending");

            var notices = await _context.Notices
                .Where(n => n.IsActive && (n.TargetRole == "All" || n.TargetRole == "Teachers"))
                .OrderByDescending(n => n.PostedDate)
                .Take(5)
                .ToListAsync();

            
            var courseAttendance = new Dictionary<string, double>();
            foreach (var course in myCourses)
            {
                var totalRecords = await _context.Attendances.CountAsync(a => a.CourseId == course.CourseId);
                var presentRecords = await _context.Attendances.CountAsync(a => a.CourseId == course.CourseId && (a.Status == "Present" || a.Status == "Late"));
                courseAttendance[course.CourseName] = totalRecords > 0 ? Math.Round((double)presentRecords / totalRecords * 100, 1) : 0;
            }

            return new TeacherDashboardViewModel
            {
                Teacher = teacher,
                MyCourses = myCourses,
                TotalStudents = totalStudents,
                TodayClasses = todayClasses,
                PendingLeaves = pendingLeaves,
                RecentNotices = notices,
                CourseAttendance = courseAttendance
            };
        }

        public async Task<Teacher> CreateTeacherAsync(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }
        }
    }
}
