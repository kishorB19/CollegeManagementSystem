using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Services
{
    public class FeeService : IFeeService
    {
        private readonly ApplicationDbContext _context;

        public FeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FeeRecord>> GetFeesByStudentAsync(int studentId)
        {
            return await _context.FeeRecords
                .Where(f => f.StudentId == studentId)
                .OrderByDescending(f => f.DueDate)
                .ToListAsync();
        }

        public async Task UpdatePaymentStatusAsync(int feeId, string status, string? transactionId = null)
        {
            var fee = await _context.FeeRecords.FindAsync(feeId);
            if (fee == null) throw new ArgumentException("Fee record not found.", nameof(feeId));

            fee.Status = status;
            if (status == "Paid")
            {
                fee.PaidDate = DateTime.UtcNow;
                fee.TransactionId = transactionId ?? $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}";
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<FeeRecord>> GetOverdueFeesAsync()
        {
            
            
            
            
            return await _context.FeeRecords
                .Include(f => f.Student)
                    .ThenInclude(s => s!.User)
                .Where(f => f.Status != "Paid" && f.DueDate < DateTime.UtcNow)
                .OrderBy(f => f.DueDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCollectedAsync()
        {
            var amounts = await _context.FeeRecords
                .Where(f => f.Status == "Paid")
                .Select(f => f.Amount)
                .ToListAsync();
            return amounts.Sum();
        }

        public async Task<decimal> GetTotalPendingAsync()
        {
            var amounts = await _context.FeeRecords
                .Where(f => f.Status != "Paid")
                .Select(f => f.Amount)
                .ToListAsync();
            return amounts.Sum();
        }

        public async Task CreateFeeRecordAsync(FeeRecord feeRecord)
        {
            _context.FeeRecords.Add(feeRecord);
            await _context.SaveChangesAsync();
        }
    }
}
