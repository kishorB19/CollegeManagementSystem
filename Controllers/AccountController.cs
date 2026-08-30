using CollegeManagementSystem.Models;
using CollegeManagementSystem.Models.ViewModels;
using CollegeManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? email = null, string? password = null, string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login", new { email, password, returnUrl });
                }
                return await RedirectToRoleDashboard();
            }
            ViewData["ReturnUrl"] = returnUrl;
            var model = new LoginViewModel
            {
                Email = email ?? string.Empty,
                Password = password ?? string.Empty
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully.", model.Email);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return await RedirectToRoleDashboard(user);
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}. Check database permissions.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Passwords do not match.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Department = model.Department,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    _logger.LogInformation("New user {Email} registered.", model.Email);
                    
                    var student = new Student
                    {
                        UserId = user.Id,
                        RollNumber = "STU" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                        Department = model.Department ?? "General",
                        Semester = 1,
                        DateOfBirth = DateTime.UtcNow.AddYears(-18),
                        AdmissionDate = DateTime.UtcNow
                    };
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Dashboard", "Student");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during registration.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task<IActionResult> RedirectToRoleDashboard(ApplicationUser? user = null)
        {
            if (User.IsInRole("Admin") || (user != null && await _userManager.IsInRoleAsync(user, "Admin")))
                return RedirectToAction("Dashboard", "Admin");
            if (User.IsInRole("Teacher") || (user != null && await _userManager.IsInRoleAsync(user, "Teacher")))
                return RedirectToAction("Dashboard", "Teacher");
            return RedirectToAction("Dashboard", "Student");
        }
    }
}
