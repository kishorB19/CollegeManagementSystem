using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class ResultService : IResultService
    {
        private readonly ApplicationDbContext _context;

        public ResultService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddResultsAsync(List<ExamResult> results)
        {
            try
            {
                
                foreach (var result in results)
                {
                    var percentage = (result.MarksObtained / result.TotalMarks) * 100;
                    result.Grade = percentage switch
                    {
                        >= 90 => "A+",
                        >= 80 => "A",
                        >= 70 => "B+",
                        >= 60 => "B",
                        >= 50 => "C",
                        >= 40 => "D",
                        _ => "F"
                    };
                }

                _context.ExamResults.AddRange(results);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to save exam results.", ex);
            }
        }

        public async Task<List<ExamResult>> GetResultsByStudentAsync(int studentId)
        {
            
            return await _context.ExamResults
                .Include(r => r.Course)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.ExamDate)
                .ToListAsync();
        }

        public async Task<List<ExamResult>> GetResultsByCourseAsync(int courseId, string? examType = null)
        {
            var query = _context.ExamResults
                .Include(r => r.Student)
                    .ThenInclude(s => s!.User)
                .Where(r => r.CourseId == courseId);

            if (!string.IsNullOrEmpty(examType))
                query = query.Where(r => r.ExamType == examType);

            return await query.OrderBy(r => r.Student!.RollNumber).ToListAsync();
        }

        public async Task<double> CalculateGPAAsync(int studentId)
        {
            
            var results = await _context.ExamResults
                .Include(r => r.Course)
                .Where(r => r.StudentId == studentId)
                .ToListAsync();

            if (!results.Any()) return 0;

            double totalPoints = 0;
            int totalCredits = 0;

            
            var courseResults = results.GroupBy(r => r.CourseId)
                .Select(g => g.OrderByDescending(r => r.ExamDate).First())
                .ToList();

            foreach (var result in courseResults)
            {
                var credits = result.Course?.Credits ?? 3;
                var gradePoint = result.Grade switch
                {
                    "A+" => 10.0,
                    "A" => 9.0,
                    "B+" => 8.0,
                    "B" => 7.0,
                    "C" => 6.0,
                    "D" => 5.0,
                    _ => 0.0
                };

                totalPoints += gradePoint * credits;
                totalCredits += credits;
            }

            return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
        }

        public async Task UpdateResultAsync(ExamResult result)
        {
            var percentage = (result.MarksObtained / result.TotalMarks) * 100;
            result.Grade = percentage switch
            {
                >= 90 => "A+",
                >= 80 => "A",
                >= 70 => "B+",
                >= 60 => "B",
                >= 50 => "C",
                >= 40 => "D",
                _ => "F"
            };

            _context.ExamResults.Update(result);
            await _context.SaveChangesAsync();
        }
    }
}
