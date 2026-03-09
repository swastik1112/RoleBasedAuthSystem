using System.ComponentModel.DataAnnotations;

namespace RoleBasedAuthSystem.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePictureFile { get; set; }  // For upload

        // Display only (not for binding)
        public string? ProfilePictureUrl { get; set; }  // We'll generate this
    }
}
