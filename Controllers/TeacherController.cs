using CollegeManagementSystem.Models;
using CollegeManagementSystem.Models.ViewModels;
using CollegeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagementSystem.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IAttendanceService _attendanceService;
        private readonly IResultService _resultService;
        private readonly IStudentService _studentService;
        private readonly INoticeService _noticeService;
        private readonly ILeaveService _leaveService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(
            ITeacherService teacherService,
            ICourseService courseService,
            IAttendanceService attendanceService,
            IResultService resultService,
            IStudentService studentService,
            INoticeService noticeService,
            ILeaveService leaveService,
            UserManager<ApplicationUser> userManager,
            ILogger<TeacherController> logger)
        {
            _teacherService = teacherService;
            _courseService = courseService;
            _attendanceService = attendanceService;
            _resultService = resultService;
            _studentService = studentService;
            _noticeService = noticeService;
            _leaveService = leaveService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var teacher = await _teacherService.GetTeacherByUserIdAsync(userId!);
                if (teacher == null) return RedirectToAction("Login", "Account");

                var model = await _teacherService.GetTeacherDashboardAsync(teacher.TeacherId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teacher dashboard");
                return View("Error");
            }
        }

        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);
            var teacher = await _teacherService.GetTeacherByUserIdAsync(userId!);
            if (teacher == null) return RedirectToAction("Login", "Account");

            var courses = await _courseService.GetCoursesByTeacherAsync(teacher.TeacherId);
            return View(courses);
        }

        

        [HttpGet]
        public async Task<IActionResult> MarkAttendance(int courseId, DateTime? date = null)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            var selectedDate = date?.Date ?? DateTime.Today;

            var students = await _studentService.GetStudentsByDepartmentAsync(course.Department);
            students = students.Where(s => s.Semester == course.Semester).ToList();

            var existing = await _attendanceService.GetAttendanceByCourseAsync(courseId, selectedDate);
            var statusMap = existing.ToDictionary(a => a.StudentId, a => a.Status);

            var model = new MarkAttendanceViewModel
            {
                CourseId = courseId,
                CourseName = $"{course.CourseName} ({course.CourseCode})",
                Date = selectedDate,
                Students = students.Select(s => new StudentAttendanceEntry
                {
                    StudentId = s.StudentId,
                    StudentName = s.User?.FullName ?? "Unknown",
                    RollNumber = s.RollNumber,
                    Status = statusMap.ContainsKey(s.StudentId) ? statusMap[s.StudentId] : "Present"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(MarkAttendanceViewModel model)
        {
            if (model.Date.Date > DateTime.Today)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Cannot mark attendance for future dates." });
                }
                TempData["Error"] = "Cannot mark attendance for future dates.";
                return RedirectToAction("MarkAttendance", new { courseId = model.CourseId });
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                var teacher = await _teacherService.GetTeacherByUserIdAsync(userId!);

                var records = model.Students.Select(s => new Attendance
                {
                    StudentId = s.StudentId,
                    CourseId = model.CourseId,
                    Date = model.Date.Date,
                    Status = s.Status,
                    MarkedByTeacherId = teacher?.TeacherId
                }).ToList();

                await _attendanceService.MarkAttendanceAsync(records);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Attendance saved successfully!" });
                }

                TempData["Success"] = "Attendance marked successfully!";
                return RedirectToAction("MyCourses");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance for course {CourseId}", model.CourseId);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Failed to save attendance." });
                }

                TempData["Error"] = "Failed to mark attendance.";
                return RedirectToAction("MarkAttendance", new { courseId = model.CourseId });
            }
        }

        public async Task<IActionResult> ViewAttendance(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            var attendance = await _attendanceService.GetAttendanceByCourseAsync(courseId);
            ViewBag.CourseName = $"{course.CourseName} ({course.CourseCode})";
            ViewBag.CourseId = courseId;
            return View(attendance);
        }

        

        [HttpGet]
        public async Task<IActionResult> AddResults(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            var students = await _studentService.GetStudentsByDepartmentAsync(course.Department);
            students = students.Where(s => s.Semester == course.Semester).ToList();

            var model = new AddResultViewModel
            {
                CourseId = courseId,
                CourseName = $"{course.CourseName} ({course.CourseCode})",
                ExamType = "Midterm",
                TotalMarks = 100,
                Semester = course.Semester,
                ExamDate = DateTime.Today,
                Students = students.Select(s => new StudentResultEntry
                {
                    StudentId = s.StudentId,
                    StudentName = s.User?.FullName ?? "Unknown",
                    RollNumber = s.RollNumber,
                    MarksObtained = 0
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResults(AddResultViewModel model)
        {
            try
            {
                var results = model.Students.Select(s => new ExamResult
                {
                    StudentId = s.StudentId,
                    CourseId = model.CourseId,
                    ExamType = model.ExamType,
                    MarksObtained = s.MarksObtained,
                    TotalMarks = model.TotalMarks,
                    Semester = model.Semester,
                    ExamDate = model.ExamDate
                }).ToList();

                await _resultService.AddResultsAsync(results);
                TempData["Success"] = "Results added successfully!";
                return RedirectToAction("MyCourses");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding results for course {CourseId}", model.CourseId);
                TempData["Error"] = "Failed to add results.";
                return RedirectToAction("AddResults", new { courseId = model.CourseId });
            }
        }

        public async Task<IActionResult> ViewResults(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            var results = await _resultService.GetResultsByCourseAsync(courseId);
            ViewBag.CourseName = $"{course.CourseName} ({course.CourseCode})";
            return View(results);
        }

        

        public async Task<IActionResult> Notices()
        {
            var notices = await _noticeService.GetNoticesByRoleAsync("Teachers");
            return View(notices);
        }
    }
}
