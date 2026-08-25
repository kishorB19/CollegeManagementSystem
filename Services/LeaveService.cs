using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _context;

        public LeaveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ApplyLeaveAsync(LeaveRequest leave)
        {
            if (leave.StartDate > leave.EndDate)
                throw new ArgumentException("Start date cannot be after end date.");

            leave.Status = "Pending";
            leave.AppliedDate = DateTime.UtcNow;
            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsAsync(string? status = null)
        {
            var query = _context.LeaveRequests
                .Include(l => l.Student)
                    .ThenInclude(s => s!.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == status);

            return await query.OrderByDescending(l => l.AppliedDate).ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeavesByStudentAsync(int studentId)
        {
            return await _context.LeaveRequests
                .Where(l => l.StudentId == studentId)
                .OrderByDescending(l => l.AppliedDate)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeavesByTeacherAsync(int teacherId)
        {
            return await _context.LeaveRequests
                .Where(l => l.TeacherId == teacherId)
                .OrderByDescending(l => l.AppliedDate)
                .ToListAsync();
        }

        public async Task ApproveLeaveAsync(int leaveId, string approvedBy)
        {
            var leave = await _context.LeaveRequests.FindAsync(leaveId);
            if (leave == null) throw new ArgumentException("Leave request not found.");

            leave.Status = "Approved";
            leave.ApprovedBy = approvedBy;
            await _context.SaveChangesAsync();
        }

        public async Task RejectLeaveAsync(int leaveId, string rejectedBy)
        {
            var leave = await _context.LeaveRequests.FindAsync(leaveId);
            if (leave == null) throw new ArgumentException("Leave request not found.");

            leave.Status = "Rejected";
            leave.ApprovedBy = rejectedBy;
            await _context.SaveChangesAsync();
        }
    }
}
