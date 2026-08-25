using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                    .ThenInclude(t => t!.User)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                    .ThenInclude(t => t!.User)
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<List<Course>> GetCoursesByTeacherAsync(int teacherId)
        {
            return await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByDepartmentAndSemesterAsync(string department, int semester)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                    .ThenInclude(t => t!.User)
                .Where(c => c.Department == department && c.Semester == semester)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
        }

        public async Task CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}
