using CollegeManagementSystem.Models;
using CollegeManagementSystem.Models.ViewModels;

namespace CollegeManagementSystem.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<Student?> GetStudentByUserIdAsync(string userId);
        Task<StudentDashboardViewModel> GetStudentDashboardAsync(int studentId);
        Task<Student> CreateStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int id);
        Task<List<Student>> GetStudentsByDepartmentAsync(string department);
        Task<List<Student>> GetStudentsBySemesterAsync(int semester);
    }

    public interface ITeacherService
    {
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task<Teacher?> GetTeacherByUserIdAsync(string userId);
        Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(int teacherId);
        Task<Teacher> CreateTeacherAsync(Teacher teacher);
        Task UpdateTeacherAsync(Teacher teacher);
        Task DeleteTeacherAsync(int id);
    }

    public interface IAttendanceService
    {
        Task MarkAttendanceAsync(List<Attendance> records);
        Task<List<Attendance>> GetAttendanceByStudentAsync(int studentId);
        Task<List<Attendance>> GetAttendanceByCourseAsync(int courseId, DateTime? date = null);
        Task<double> GetAttendancePercentageAsync(int studentId, int? courseId = null);
        Task<List<AttendanceSummary>> GetAttendanceSummaryAsync(int studentId);
        Task<bool> IsAttendanceMarkedAsync(int courseId, DateTime date);
    }

    public interface IResultService
    {
        Task AddResultsAsync(List<ExamResult> results);
        Task<List<ExamResult>> GetResultsByStudentAsync(int studentId);
        Task<List<ExamResult>> GetResultsByCourseAsync(int courseId, string? examType = null);
        Task<double> CalculateGPAAsync(int studentId);
        Task UpdateResultAsync(ExamResult result);
    }

    public interface IFeeService
    {
        Task<List<FeeRecord>> GetFeesByStudentAsync(int studentId);
        Task UpdatePaymentStatusAsync(int feeId, string status, string? transactionId = null);
        Task<List<FeeRecord>> GetOverdueFeesAsync();
        Task<decimal> GetTotalCollectedAsync();
        Task<decimal> GetTotalPendingAsync();
        Task CreateFeeRecordAsync(FeeRecord feeRecord);
    }

    public interface INoticeService
    {
        Task<List<Notice>> GetAllNoticesAsync();
        Task<Notice?> GetNoticeByIdAsync(int id);
        Task<List<Notice>> GetNoticesByRoleAsync(string role);
        Task CreateNoticeAsync(Notice notice);
        Task UpdateNoticeAsync(Notice notice);
        Task DeleteNoticeAsync(int id);
    }

    public interface ILeaveService
    {
        Task ApplyLeaveAsync(LeaveRequest leave);
        Task<List<LeaveRequest>> GetLeaveRequestsAsync(string? status = null);
        Task<List<LeaveRequest>> GetLeavesByStudentAsync(int studentId);
        Task<List<LeaveRequest>> GetLeavesByTeacherAsync(int teacherId);
        Task ApproveLeaveAsync(int leaveId, string approvedBy);
        Task RejectLeaveAsync(int leaveId, string rejectedBy);
    }

    public interface IDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardAsync();
        Task<Dictionary<string, int>> GetDepartmentWiseStudentsAsync();
        Task<Dictionary<string, double>> GetMonthlyAttendanceAsync();
    }

    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<List<Course>> GetCoursesByTeacherAsync(int teacherId);
        Task<List<Course>> GetCoursesByDepartmentAndSemesterAsync(string department, int semester);
        Task CreateCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
    }
}
