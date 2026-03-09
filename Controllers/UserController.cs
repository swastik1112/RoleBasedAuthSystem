using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedAuthSystem.Models;
using RoleBasedAuthSystem.Models.ViewModels;

namespace RoleBasedAuthSystem.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                FullName = user.FullName ?? user.UserName ?? "Welcome",
                ProfilePictureUrl = user.ProfilePicture != null
                    ? Url.Action("GetProfilePicture", "User", new { id = user.Id })
                    : "/images/user.png"
            };

            return View(model);
        }

        // GET: /User/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FullName = user.FullName ?? "",
                ProfilePictureUrl = user.ProfilePicture != null
                    ? $"/User/GetProfilePicture/{user.Id}"
                    : "/images/user.png"  // fallback image in wwwroot
            };

            return View(model);
        }

        // POST: /User/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FullName = model.FullName;

            // Handle file upload if provided
            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await model.ProfilePictureFile.CopyToAsync(memoryStream);

                user.ProfilePicture = memoryStream.ToArray();
                user.ProfilePictureContentType = model.ProfilePictureFile.ContentType;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Profile picture (for <img src> tags)
        [HttpGet]
        [AllowAnonymous]  // Allow anyone to see avatars if needed
        public async Task<IActionResult> GetProfilePicture(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.ProfilePicture == null || user.ProfilePictureContentType == null)
            {
                return Redirect("/images/user.png"); // fallback
            }

            return File(user.ProfilePicture, user.ProfilePictureContentType);
        }
    }
}
