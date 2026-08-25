using CollegeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagementSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceApiController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceApiController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudentAttendance(int studentId)
        {
            try
            {
                var attendance = await _attendanceService.GetAttendanceByStudentAsync(studentId);
                var percentage = await _attendanceService.GetAttendancePercentageAsync(studentId);
                var summary = await _attendanceService.GetAttendanceSummaryAsync(studentId);

                return Ok(new
                {
                    studentId,
                    overallPercentage = percentage,
                    courseSummary = summary,
                    totalRecords = attendance.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseAttendance(int courseId, [FromQuery] DateTime? date = null)
        {
            try
            {
                var records = await _attendanceService.GetAttendanceByCourseAsync(courseId, date);
                return Ok(records.Select(r => new
                {
                    r.AttendanceId,
                    r.StudentId,
                    studentName = r.Student?.User?.FullName,
                    rollNumber = r.Student?.RollNumber,
                    r.Date,
                    r.Status
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultApiController : ControllerBase
    {
        private readonly IResultService _resultService;

        public ResultApiController(IResultService resultService)
        {
            _resultService = resultService;
        }
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudentResults(int studentId)
        {
            try
            {
                var results = await _resultService.GetResultsByStudentAsync(studentId);
                var gpa = await _resultService.CalculateGPAAsync(studentId);

                return Ok(new
                {
                    studentId,
                    gpa,
                    results = results.Select(r => new
                    {
                        r.ResultId,
                        courseName = r.Course?.CourseName,
                        courseCode = r.Course?.CourseCode,
                        r.ExamType,
                        r.MarksObtained,
                        r.TotalMarks,
                        r.Grade,
                        r.Semester,
                        r.ExamDate
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardApiController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardApiController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetAdminStats()
        {
            try
            {
                var dashboard = await _dashboardService.GetAdminDashboardAsync();
                return Ok(new
                {
                    dashboard.TotalStudents,
                    dashboard.TotalTeachers,
                    dashboard.TotalCourses,
                    dashboard.TotalNotices,
                    dashboard.TotalFeeCollected,
                    dashboard.TotalFeePending,
                    dashboard.OverallAttendancePercentage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("charts/departments")]
        public async Task<IActionResult> GetDepartmentChart()
        {
            var data = await _dashboardService.GetDepartmentWiseStudentsAsync();
            return Ok(new { labels = data.Keys, values = data.Values });
        }
        [HttpGet("charts/attendance")]
        public async Task<IActionResult> GetAttendanceChart()
        {
            var data = await _dashboardService.GetMonthlyAttendanceAsync();
            return Ok(new { labels = data.Keys, values = data.Values });
        }
    }
}
