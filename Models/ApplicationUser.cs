using Microsoft.AspNetCore.Identity;

namespace RoleBasedAuthSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]  // Helps with GDPR export if you implement later
        public string? FullName { get; set; }

        [PersonalData]
        public byte[]? ProfilePicture { get; set; }   // Store image bytes

        public string? ProfilePictureContentType { get; set; }  // e.g. "image/jpeg"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
