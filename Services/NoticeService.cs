using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class NoticeService : INoticeService
    {
        private readonly ApplicationDbContext _context;

        public NoticeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notice>> GetAllNoticesAsync()
        {
            return await _context.Notices
                .OrderByDescending(n => n.PostedDate)
                .ToListAsync();
        }

        public async Task<Notice?> GetNoticeByIdAsync(int id)
        {
            return await _context.Notices.FindAsync(id);
        }

        public async Task<List<Notice>> GetNoticesByRoleAsync(string role)
        {
            return await _context.Notices
                .Where(n => n.IsActive && (n.TargetRole == "All" || n.TargetRole == role))
                .OrderByDescending(n => n.PostedDate)
                .ToListAsync();
        }

        public async Task CreateNoticeAsync(Notice notice)
        {
            notice.PostedDate = DateTime.UtcNow;
            _context.Notices.Add(notice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNoticeAsync(Notice notice)
        {
            _context.Notices.Update(notice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoticeAsync(int id)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice != null)
            {
                _context.Notices.Remove(notice);
                await _context.SaveChangesAsync();
            }
        }
    }
}
