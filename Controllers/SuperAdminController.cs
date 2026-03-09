using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoleBasedAuthSystem.Data;
using RoleBasedAuthSystem.Models;
using RoleBasedAuthSystem.Models.ViewModels;

namespace RoleBasedAuthSystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;     // optional - for more complex queries

        public SuperAdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var allUsers = await _userManager.Users.ToListAsync();

            var model = new SuperAdminDashboardViewModel
            {
                TotalUsers = allUsers.Count,

                SuperAdminCount = (await _userManager.GetUsersInRoleAsync("SuperAdmin")).Count,
                AdminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count,
                UserCount = (await _userManager.GetUsersInRoleAsync("User")).Count,

                // New this month (example: registered >= first day of current month)
                NewThisMonth = allUsers
                    .Count(u => u.CreatedAt >= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)),

                // If you added LockoutEnabled / LockoutEnd to ApplicationUser or use Identity defaults
                LockedAccounts = allUsers.Count(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow)
            };

            return View(model);
        }

        // Controllers/SuperAdminController.cs

        [HttpGet]
        public async Task<IActionResult> ManageUsers(string searchTerm = "", int page = 1)
        {
            const int pageSize = 10;

            // Base query
            var query = _userManager.Users.AsQueryable();

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(searchTerm));
            }

            // Get total count for pagination
            int totalCount = await query.CountAsync();

            // Get paginated data
            var users = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "None";

                viewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = role,
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
                });
            }

            var model = new UserListViewModel
            {
                Users = viewModels,
                SearchTerm = searchTerm,
                CurrentPage = page,
                PageSize = pageSize,
                TotalUsers = totalCount
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel { Id = user.Id, Email = user.Email, Role = roles.FirstOrDefault() };
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);

                return RedirectToAction("ManageUsers");
            }
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Prevent deleting self or other SuperAdmins? Add logic if needed.
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id, string returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToLocal(returnUrl);
            }

            // Optional: Prevent locking yourself
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "You cannot lock your own account.";
                return RedirectToLocal(returnUrl);
            }

            // Lock for a long time (e.g. 1 year) or permanent until unlocked
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(10); // very long lockout

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.Email} has been locked.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to lock user.";
            }

            return RedirectToLocal(returnUrl ?? Url.Action("ManageUsers"));
        }

        // POST: Unlock user account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id, string returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToLocal(returnUrl);
            }

            user.LockoutEnd = null;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.Email} has been unlocked.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unlock user.";
            }

            return RedirectToLocal(returnUrl ?? Url.Action("ManageUsers"));
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("ManageUsers");
        }
        private async Task<bool> IsUserLocked(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.LockoutEnd.HasValue == true && user.LockoutEnd > DateTimeOffset.UtcNow;
        }


        // Access other dashboards (full control)
        public IActionResult AdminDashboard() => RedirectToAction("Dashboard", "Admin");
        public IActionResult UserDashboard() => RedirectToAction("Dashboard", "User");
    }
}
