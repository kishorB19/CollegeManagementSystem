using CollegeManagementSystem.Models;
using CollegeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IFeeService _feeService;
        private readonly INoticeService _noticeService;
        private readonly ILeaveService _leaveService;
        private readonly IAttendanceService _attendanceService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IDashboardService dashboardService,
            IStudentService studentService,
            ITeacherService teacherService,
            ICourseService courseService,
            IFeeService feeService,
            INoticeService noticeService,
            ILeaveService leaveService,
            IAttendanceService attendanceService,
            UserManager<ApplicationUser> userManager,
            ILogger<AdminController> logger)
        {
            _dashboardService = dashboardService;
            _studentService = studentService;
            _teacherService = teacherService;
            _courseService = courseService;
            _feeService = feeService;
            _noticeService = noticeService;
            _leaveService = leaveService;
            _attendanceService = attendanceService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var model = await _dashboardService.GetAdminDashboardAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                return View("Error");
            }
        }

        

        public async Task<IActionResult> Students()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);
                if (student != null && student.User != null)
                {
                    await _studentService.DeleteStudentAsync(id);
                    await _userManager.DeleteAsync(student.User);
                    TempData["Success"] = "Student deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student {Id}", id);
                TempData["Error"] = "Failed to delete student.";
            }
            return RedirectToAction("Students");
        }

        

        public async Task<IActionResult> Teachers()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return View(teachers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherByIdAsync(id);
                if (teacher != null && teacher.User != null)
                {
                    await _teacherService.DeleteTeacherAsync(id);
                    await _userManager.DeleteAsync(teacher.User);
                    TempData["Success"] = "Teacher deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher {Id}", id);
                TempData["Error"] = "Failed to delete teacher.";
            }
            return RedirectToAction("Teachers");
        }

        

        public async Task<IActionResult> Courses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            try
            {
                await _courseService.CreateCourseAsync(course);
                TempData["Success"] = "Course created successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                TempData["Error"] = "Failed to create course.";
            }
            return RedirectToAction("Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                TempData["Success"] = "Course deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {Id}", id);
                TempData["Error"] = "Failed to delete course.";
            }
            return RedirectToAction("Courses");
        }

        

        public async Task<IActionResult> Fees()
        {
            var overdues = await _feeService.GetOverdueFeesAsync();
            ViewBag.TotalCollected = await _feeService.GetTotalCollectedAsync();
            ViewBag.TotalPending = await _feeService.GetTotalPendingAsync();
            return View(overdues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkFeePaid(int feeId)
        {
            try
            {
                await _feeService.UpdatePaymentStatusAsync(feeId, "Paid");
                TempData["Success"] = "Fee marked as paid.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking fee paid {Id}", feeId);
                TempData["Error"] = "Failed to update fee status.";
            }
            return RedirectToAction("Fees");
        }

        

        public async Task<IActionResult> Notices()
        {
            var notices = await _noticeService.GetAllNoticesAsync();
            return View(notices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNotice(Notice notice)
        {
            try
            {
                notice.PostedBy = "Admin";
                await _noticeService.CreateNoticeAsync(notice);
                TempData["Success"] = "Notice posted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notice");
                TempData["Error"] = "Failed to post notice.";
            }
            return RedirectToAction("Notices");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotice(int id)
        {
            try
            {
                await _noticeService.DeleteNoticeAsync(id);
                TempData["Success"] = "Notice deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notice {Id}", id);
                TempData["Error"] = "Failed to delete notice.";
            }
            return RedirectToAction("Notices");
        }

        

        public async Task<IActionResult> Leaves()
        {
            var leaves = await _leaveService.GetLeaveRequestsAsync();
            return View(leaves);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            try
            {
                await _leaveService.ApproveLeaveAsync(id, "Admin");
                TempData["Success"] = "Leave request approved.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave {Id}", id);
                TempData["Error"] = "Failed to approve leave.";
            }
            return RedirectToAction("Leaves");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLeave(int id)
        {
            try
            {
                await _leaveService.RejectLeaveAsync(id, "Admin");
                TempData["Success"] = "Leave request rejected.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave {Id}", id);
                TempData["Error"] = "Failed to reject leave.";
            }
            return RedirectToAction("Leaves");
        }
    }
}
