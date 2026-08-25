using CollegeManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            
            await context.Database.EnsureCreatedAsync();

            
            string[] roles = { "Admin", "Teacher", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            
            if (await userManager.FindByEmailAsync("admin@college.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@college.com",
                    Email = "admin@college.com",
                    FullName = "System Administrator",
                    Department = "Administration",
                    EmailConfirmed = true,
                    IsActive = true
                };
                await userManager.CreateAsync(admin, "admin123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            
            if (await context.Teachers.AnyAsync()) return;

            
            var teacherUsers = new[]
            {
                new { Email = "kishor.kumar@college.com", Name = "Dr. Kishor Kumar", Dept = "Computer Science", EmpId = "TCH001", Designation = "Professor", Specialization = "Machine Learning", Qualification = "Ph.D. Computer Science", Phone = "9876543001" },
                new { Email = "sarita.sharma@college.com", Name = "Prof. Sarita Sharma", Dept = "Mathematics", EmpId = "TCH002", Designation = "Associate Professor", Specialization = "Applied Mathematics", Qualification = "Ph.D. Mathematics", Phone = "9876543002" },
                new { Email = "manoj.patel@college.com", Name = "Dr. Manoj Patel", Dept = "Computer Science", EmpId = "TCH003", Designation = "Assistant Professor", Specialization = "Database Systems", Qualification = "Ph.D. Information Technology", Phone = "9876543003" },
                new { Email = "ekta.gupta@college.com", Name = "Prof. Ekta Gupta", Dept = "Electronics", EmpId = "TCH004", Designation = "Professor", Specialization = "VLSI Design", Qualification = "Ph.D. Electronics Engineering", Phone = "9876543004" },
                new { Email = "rajesh.verma@college.com", Name = "Dr. Rajesh Verma", Dept = "Physics", EmpId = "TCH005", Designation = "Associate Professor", Specialization = "Quantum Mechanics", Qualification = "Ph.D. Physics", Phone = "9876543005" }
            };

            var teachers = new List<Teacher>();
            foreach (var t in teacherUsers)
            {
                var user = new ApplicationUser
                {
                    UserName = t.Email,
                    Email = t.Email,
                    FullName = t.Name,
                    Department = t.Dept,
                    EmailConfirmed = true,
                    IsActive = true
                };
                await userManager.CreateAsync(user, "teacher123");
                await userManager.AddToRoleAsync(user, "Teacher");

                var teacher = new Teacher
                {
                    UserId = user.Id,
                    EmployeeId = t.EmpId,
                    Department = t.Dept,
                    Designation = t.Designation,
                    Specialization = t.Specialization,
                    Qualification = t.Qualification,
                    Phone = t.Phone,
                    JoinDate = DateTime.UtcNow.AddYears(-3)
                };
                teachers.Add(teacher);
            }
            context.Teachers.AddRange(teachers);
            await context.SaveChangesAsync();

            
            var courses = new List<Course>
            {
                new() { CourseName = "Data Structures & Algorithms", CourseCode = "CS301", Department = "Computer Science", Semester = 3, Credits = 4, TeacherId = teachers[0].TeacherId },
                new() { CourseName = "Database Management Systems", CourseCode = "CS302", Department = "Computer Science", Semester = 3, Credits = 4, TeacherId = teachers[2].TeacherId },
                new() { CourseName = "Web Development", CourseCode = "CS303", Department = "Computer Science", Semester = 3, Credits = 3, TeacherId = teachers[0].TeacherId },
                new() { CourseName = "Engineering Mathematics III", CourseCode = "MA301", Department = "Mathematics", Semester = 3, Credits = 4, TeacherId = teachers[1].TeacherId },
                new() { CourseName = "Digital Electronics", CourseCode = "EC301", Department = "Electronics", Semester = 3, Credits = 3, TeacherId = teachers[3].TeacherId },
                new() { CourseName = "Operating Systems", CourseCode = "CS401", Department = "Computer Science", Semester = 4, Credits = 4, TeacherId = teachers[2].TeacherId },
                new() { CourseName = "Computer Networks", CourseCode = "CS402", Department = "Computer Science", Semester = 4, Credits = 3, TeacherId = teachers[0].TeacherId },
                new() { CourseName = "Applied Physics", CourseCode = "PH301", Department = "Physics", Semester = 3, Credits = 3, TeacherId = teachers[4].TeacherId }
            };
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            
            var studentData = new[]
            {
                new { Email = "rahul.kumar@student.college.com", Name = "Rahul Kumar", Roll = "2024CS001", Dept = "Computer Science", Sem = 3, Sec = "A", Phone = "9876543101", Guardian = "Suresh Kumar", GPhone = "9876543201", Dob = new DateTime(2004, 5, 15) },
                new { Email = "priya.sharma@student.college.com", Name = "Priya Sharma", Roll = "2024CS002", Dept = "Computer Science", Sem = 3, Sec = "A", Phone = "9876543102", Guardian = "Rajesh Sharma", GPhone = "9876543202", Dob = new DateTime(2004, 3, 22) },
                new { Email = "amit.patel@student.college.com", Name = "Amit Patel", Roll = "2024CS003", Dept = "Computer Science", Sem = 3, Sec = "B", Phone = "9876543103", Guardian = "Vikram Patel", GPhone = "9876543203", Dob = new DateTime(2004, 8, 10) },
                new { Email = "sneha.gupta@student.college.com", Name = "Sneha Gupta", Roll = "2024CS004", Dept = "Computer Science", Sem = 3, Sec = "B", Phone = "9876543104", Guardian = "Manoj Gupta", GPhone = "9876543204", Dob = new DateTime(2004, 1, 28) },
                new { Email = "vikash.singh@student.college.com", Name = "Vikash Singh", Roll = "2024CS005", Dept = "Computer Science", Sem = 4, Sec = "A", Phone = "9876543105", Guardian = "Ramesh Singh", GPhone = "9876543205", Dob = new DateTime(2003, 11, 5) },
                new { Email = "neha.verma@student.college.com", Name = "Neha Verma", Roll = "2024EC001", Dept = "Electronics", Sem = 3, Sec = "A", Phone = "9876543106", Guardian = "Ashok Verma", GPhone = "9876543206", Dob = new DateTime(2004, 7, 19) },
                new { Email = "arjun.reddy@student.college.com", Name = "Arjun Reddy", Roll = "2024EC002", Dept = "Electronics", Sem = 3, Sec = "A", Phone = "9876543107", Guardian = "Krishna Reddy", GPhone = "9876543207", Dob = new DateTime(2004, 4, 30) },
                new { Email = "ananya.das@student.college.com", Name = "Ananya Das", Roll = "2024PH001", Dept = "Physics", Sem = 3, Sec = "A", Phone = "9876543108", Guardian = "Soumya Das", GPhone = "9876543208", Dob = new DateTime(2004, 9, 12) }
            };

            var students = new List<Student>();
            foreach (var s in studentData)
            {
                var user = new ApplicationUser
                {
                    UserName = s.Email,
                    Email = s.Email,
                    FullName = s.Name,
                    Department = s.Dept,
                    EmailConfirmed = true,
                    IsActive = true
                };
                await userManager.CreateAsync(user, "student123");
                await userManager.AddToRoleAsync(user, "Student");

                var student = new Student
                {
                    UserId = user.Id,
                    RollNumber = s.Roll,
                    Department = s.Dept,
                    Semester = s.Sem,
                    Section = s.Sec,
                    DateOfBirth = s.Dob,
                    Phone = s.Phone,
                    GuardianName = s.Guardian,
                    GuardianPhone = s.GPhone,
                    Address = "123 College Road, City",
                    AdmissionDate = DateTime.UtcNow.AddYears(-2)
                };
                students.Add(student);
            }
            context.Students.AddRange(students);
            await context.SaveChangesAsync();

            
            var random = new Random(42);
            var attendanceRecords = new List<Attendance>();
            var today = DateTime.UtcNow.Date;

            foreach (var student in students.Where(s => s.Semester == 3 && s.Department == "Computer Science"))
            {
                var semCourses = courses.Where(c => c.Semester == 3 && c.Department == "Computer Science").ToList();
                foreach (var course in semCourses)
                {
                    for (int day = 30; day >= 1; day--)
                    {
                        var date = today.AddDays(-day);
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                        var statuses = new[] { "Present", "Present", "Present", "Present", "Absent", "Late" };
                        attendanceRecords.Add(new Attendance
                        {
                            StudentId = student.StudentId,
                            CourseId = course.CourseId,
                            Date = date,
                            Status = statuses[random.Next(statuses.Length)],
                            MarkedByTeacherId = course.TeacherId
                        });
                    }
                }
            }
            context.Attendances.AddRange(attendanceRecords);
            await context.SaveChangesAsync();

            
            var examResults = new List<ExamResult>();
            foreach (var student in students.Where(s => s.Semester == 3 && s.Department == "Computer Science"))
            {
                var semCourses = courses.Where(c => c.Semester == 3 && c.Department == "Computer Science").ToList();
                foreach (var course in semCourses)
                {
                    var marks = 40 + random.Next(60);
                    var grade = marks >= 90 ? "A+" : marks >= 80 ? "A" : marks >= 70 ? "B+" : marks >= 60 ? "B" : marks >= 50 ? "C" : "F";
                    examResults.Add(new ExamResult
                    {
                        StudentId = student.StudentId,
                        CourseId = course.CourseId,
                        ExamType = "Midterm",
                        MarksObtained = marks,
                        TotalMarks = 100,
                        Grade = grade,
                        Semester = 3,
                        ExamDate = today.AddDays(-15)
                    });
                }
            }
            context.ExamResults.AddRange(examResults);
            await context.SaveChangesAsync();

            
            var feeTypes = new[] { ("Tuition", 45000m), ("Library", 2000m), ("Lab", 5000m), ("Exam", 3000m) };
            var feeRecords = new List<FeeRecord>();
            foreach (var student in students)
            {
                foreach (var (feeType, amount) in feeTypes)
                {
                    var isPaid = random.Next(10) > 3; 
                    feeRecords.Add(new FeeRecord
                    {
                        StudentId = student.StudentId,
                        FeeType = feeType,
                        Amount = amount,
                        DueDate = today.AddDays(-10),
                        PaidDate = isPaid ? today.AddDays(-random.Next(1, 15)) : null,
                        Status = isPaid ? "Paid" : (today > today.AddDays(-10) ? "Overdue" : "Pending"),
                        TransactionId = isPaid ? $"TXN{random.Next(100000, 999999)}" : null,
                        Semester = student.Semester
                    });
                }
            }
            context.FeeRecords.AddRange(feeRecords);
            await context.SaveChangesAsync();

            
            var notices = new List<Notice>
            {
                new() { Title = "Mid-Semester Examination Schedule", Content = "Mid-semester examinations will commence from next Monday. All students are requested to check their respective timetables on the notice board. Please carry your ID cards to the examination hall.", PostedBy = "Admin", TargetRole = "All", Priority = "High", PostedDate = today.AddDays(-5) },
                new() { Title = "Annual Sports Day Registration", Content = "Registration for Annual Sports Day is now open. Students interested in participating should register at the Sports Office by Friday. Events include athletics, cricket, football, badminton, and chess.", PostedBy = "Admin", TargetRole = "Students", Priority = "Normal", PostedDate = today.AddDays(-3) },
                new() { Title = "Faculty Development Program", Content = "A three-day Faculty Development Program on 'Emerging Trends in AI and Machine Learning' will be held next week. All faculty members are encouraged to attend and register through the academic portal.", PostedBy = "Admin", TargetRole = "Teachers", Priority = "Normal", PostedDate = today.AddDays(-2) },
                new() { Title = "Library Renovation Notice", Content = "The central library will remain closed for renovation from this Saturday for one week. Digital library services will continue to be available online. Please return all borrowed books before the closure date.", PostedBy = "Admin", TargetRole = "All", Priority = "High", PostedDate = today.AddDays(-1) },
                new() { Title = "Scholarship Applications Open", Content = "Applications for merit-based scholarships for the current academic year are now being accepted. Eligible students with CGPA above 8.0 may apply through the Student Affairs office. Last date: end of this month.", PostedBy = "Admin", TargetRole = "Students", Priority = "Urgent", PostedDate = today }
            };
            context.Notices.AddRange(notices);
            await context.SaveChangesAsync();

            
            var leaveRequests = new List<LeaveRequest>
            {
                new() { StudentId = students[0].StudentId, LeaveType = "Sick", StartDate = today.AddDays(-5), EndDate = today.AddDays(-3), Reason = "High fever and flu symptoms. Doctor advised 3 days of rest.", Status = "Approved", ApprovedBy = "Dr. Kishor Kumar" },
                new() { StudentId = students[1].StudentId, LeaveType = "Personal", StartDate = today.AddDays(2), EndDate = today.AddDays(4), Reason = "Family function — sister's wedding ceremony at hometown.", Status = "Pending" },
                new() { StudentId = students[2].StudentId, LeaveType = "Academic", StartDate = today.AddDays(-1), EndDate = today, Reason = "Attending a national level coding competition at IIT Delhi.", Status = "Approved", ApprovedBy = "Dr. Manoj Patel" }
            };
            context.LeaveRequests.AddRange(leaveRequests);
            await context.SaveChangesAsync();

            
            var timetableEntries = new List<Timetable>
            {
                new() { CourseId = courses[0].CourseId, Day = "Monday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Room = "CS-101" },
                new() { CourseId = courses[1].CourseId, Day = "Monday", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), Room = "CS-102" },
                new() { CourseId = courses[2].CourseId, Day = "Monday", StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 0, 0), Room = "CS-Lab1" },
                new() { CourseId = courses[3].CourseId, Day = "Monday", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 0, 0), Room = "MA-201" },
                new() { CourseId = courses[0].CourseId, Day = "Tuesday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Room = "CS-101" },
                new() { CourseId = courses[4].CourseId, Day = "Tuesday", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), Room = "EC-101" },
                new() { CourseId = courses[1].CourseId, Day = "Tuesday", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 0, 0), Room = "CS-102" },
                new() { CourseId = courses[2].CourseId, Day = "Wednesday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Room = "CS-Lab1" },
                new() { CourseId = courses[3].CourseId, Day = "Wednesday", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), Room = "MA-201" },
                new() { CourseId = courses[0].CourseId, Day = "Wednesday", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 0, 0), Room = "CS-101" },
                new() { CourseId = courses[1].CourseId, Day = "Thursday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Room = "CS-102" },
                new() { CourseId = courses[4].CourseId, Day = "Thursday", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), Room = "EC-101" },
                new() { CourseId = courses[7].CourseId, Day = "Thursday", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 0, 0), Room = "PH-101" },
                new() { CourseId = courses[2].CourseId, Day = "Friday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Room = "CS-Lab1" },
                new() { CourseId = courses[3].CourseId, Day = "Friday", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), Room = "MA-201" },
                new() { CourseId = courses[7].CourseId, Day = "Friday", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 0, 0), Room = "PH-101" }
            };
            context.Timetables.AddRange(timetableEntries);
            await context.SaveChangesAsync();
        }
    }
}
