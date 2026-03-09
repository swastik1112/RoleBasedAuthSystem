using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedAuthSystem.Models;
using RoleBasedAuthSystem.Models.ViewModels;

namespace RoleBasedAuthSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();
                if (role == "User")  // Admins can only manage Users
                {
                    userViewModels.Add(new UserViewModel { Id = user.Id, Email = user.Email, Role = role });
                }
            }
            return View(userViewModels);  // Reuse SuperAdmin's ManageUsers view or create separate
        }


        // Similar EditUser and DeleteUser as SuperAdmin, but restrict to Users only and no role changes to SuperAdmin/Admin
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() != "User") return Forbid();  // Cannot edit non-Users
            var model = new EditUserViewModel { Id = user.Id, Email = user.Email, Role = "User" };  // Fixed to User
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null || (await _userManager.GetRolesAsync(user)).FirstOrDefault() != "User") return Forbid();

                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("ManageUsers");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && (await _userManager.GetRolesAsync(user)).FirstOrDefault() == "User")
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id, string returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            // Admin can only lock regular Users
            if (role != "User")
            {
                TempData["ErrorMessage"] = "Admins can only lock regular User accounts.";
                return RedirectToLocal(returnUrl);
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(10);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id, string returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() != "User")
            {
                TempData["ErrorMessage"] = "Admins can only unlock regular User accounts.";
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

    }
}
