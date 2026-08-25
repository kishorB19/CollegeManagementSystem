using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using CollegeManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task MarkAttendanceAsync(List<Attendance> records)
        {
            try
            {
                
                var firstRecord = records.FirstOrDefault();
                if (firstRecord != null)
                {
                    var existing = await _context.Attendances
                        .Where(a => a.CourseId == firstRecord.CourseId && a.Date == firstRecord.Date)
                        .ToListAsync();
                    _context.Attendances.RemoveRange(existing);
                }

                _context.Attendances.AddRange(records);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to mark attendance. Duplicate entry may exist.", ex);
            }
        }

        public async Task<List<Attendance>> GetAttendanceByStudentAsync(int studentId)
        {
            
            return await _context.Attendances
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetAttendanceByCourseAsync(int courseId, DateTime? date = null)
        {
            
            var query = _context.Attendances
                .Include(a => a.Student)
                    .ThenInclude(s => s!.User)
                .Where(a => a.CourseId == courseId);

            if (date.HasValue)
                query = query.Where(a => a.Date == date.Value.Date);

            return await query.OrderBy(a => a.Student!.RollNumber).ToListAsync();
        }

        public async Task<double> GetAttendancePercentageAsync(int studentId, int? courseId = null)
        {
            
            
            var query = _context.Attendances.Where(a => a.StudentId == studentId);
            if (courseId.HasValue)
                query = query.Where(a => a.CourseId == courseId.Value);

            var total = await query.CountAsync();
            if (total == 0) return 0;

            var present = await query.CountAsync(a => a.Status == "Present" || a.Status == "Late");
            return Math.Round((double)present / total * 100, 1);
        }

        public async Task<List<AttendanceSummary>> GetAttendanceSummaryAsync(int studentId)
        {
            
            
            
            
            var attendances = await _context.Attendances
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();

            return attendances
                .GroupBy(a => a.CourseId)
                .Select(g =>
                {
                    var course = g.First().Course!;
                    var total = g.Count();
                    var present = g.Count(a => a.Status == "Present" || a.Status == "Late");
                    return new AttendanceSummary
                    {
                        CourseName = course.CourseName,
                        CourseCode = course.CourseCode,
                        TotalClasses = total,
                        PresentClasses = present,
                        Percentage = total > 0 ? Math.Round((double)present / total * 100, 1) : 0
                    };
                })
                .ToList();
        }

        public async Task<bool> IsAttendanceMarkedAsync(int courseId, DateTime date)
        {
            return await _context.Attendances
                .AnyAsync(a => a.CourseId == courseId && a.Date == date.Date);
        }
    }
}
