using System.ComponentModel.DataAnnotations;

namespace RoleBasedAuthSystem.Models.ViewModels
{
    public class ChangeProfilePictureViewModel
    {
        [Required]
        [Display(Name = "New Profile Picture")]
        public IFormFile ProfilePictureFile { get; set; }
    }
}
