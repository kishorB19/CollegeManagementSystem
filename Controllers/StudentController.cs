using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models;
using CollegeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IAttendanceService _attendanceService;
        private readonly IResultService _resultService;
        private readonly IFeeService _feeService;
        private readonly ILeaveService _leaveService;
        private readonly INoticeService _noticeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            IStudentService studentService,
            IAttendanceService attendanceService,
            IResultService resultService,
            IFeeService feeService,
            ILeaveService leaveService,
            INoticeService noticeService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _attendanceService = attendanceService;
            _resultService = resultService;
            _feeService = feeService;
            _leaveService = leaveService;
            _noticeService = noticeService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var student = await _studentService.GetStudentByUserIdAsync(userId!);
                if (student == null) return RedirectToAction("Login", "Account");

                var model = await _studentService.GetStudentDashboardAsync(student.StudentId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student dashboard");
                return View("Error");
            }
        }

        public async Task<IActionResult> Attendance()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentService.GetStudentByUserIdAsync(userId!);
            if (student == null) return RedirectToAction("Login", "Account");

            var summary = await _attendanceService.GetAttendanceSummaryAsync(student.StudentId);
            ViewBag.OverallAttendance = await _attendanceService.GetAttendancePercentageAsync(student.StudentId);
            return View(summary);
        }

        public async Task<IActionResult> Results()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentService.GetStudentByUserIdAsync(userId!);
            if (student == null) return RedirectToAction("Login", "Account");

            var results = await _resultService.GetResultsByStudentAsync(student.StudentId);
            ViewBag.GPA = await _resultService.CalculateGPAAsync(student.StudentId);
            return View(results);
        }

        public async Task<IActionResult> Fees()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentService.GetStudentByUserIdAsync(userId!);
            if (student == null) return RedirectToAction("Login", "Account");

            var fees = await _feeService.GetFeesByStudentAsync(student.StudentId);
            return View(fees);
        }

        

        public async Task<IActionResult> Leave()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentService.GetStudentByUserIdAsync(userId!);
            if (student == null) return RedirectToAction("Login", "Account");

            var leaves = await _leaveService.GetLeavesByStudentAsync(student.StudentId);
            return View(leaves);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyLeave(string leaveType, DateTime startDate, DateTime endDate, string reason)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var student = await _studentService.GetStudentByUserIdAsync(userId!);
                if (student == null) return RedirectToAction("Login", "Account");

                var leave = new LeaveRequest
                {
                    StudentId = student.StudentId,
                    LeaveType = leaveType,
                    StartDate = startDate,
                    EndDate = endDate,
                    Reason = reason
                };

                await _leaveService.ApplyLeaveAsync(leave);
                TempData["Success"] = "Leave request submitted successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying leave");
                TempData["Error"] = "Failed to submit leave request.";
            }
            return RedirectToAction("Leave");
        }

        

        public async Task<IActionResult> Notices()
        {
            var notices = await _noticeService.GetNoticesByRoleAsync("Students");
            return View(notices);
        }

        public async Task<IActionResult> Timetable()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentService.GetStudentByUserIdAsync(userId!);
            if (student == null) return RedirectToAction("Login", "Account");

            var timetable = await _context.Timetables
                .Include(t => t.Course)
                    .ThenInclude(c => c!.Teacher)
                        .ThenInclude(t => t!.User)
                .Where(t => t.Course!.Department == student.Department && t.Course.Semester == student.Semester)
                .OrderBy(t => t.Day)
                .ThenBy(t => t.StartTime)
                .ToListAsync();

            return View(timetable);
        }
    }
}
