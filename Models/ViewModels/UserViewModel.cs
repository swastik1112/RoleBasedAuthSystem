namespace RoleBasedAuthSystem.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        // ← Add these two new properties
        public bool IsLocked { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
