using System.ComponentModel.DataAnnotations;

namespace RoleBasedAuthSystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }  // For role assignment
    }
   
}
