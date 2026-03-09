namespace RoleBasedAuthSystem.Models.ViewModels
{
    // Models/ViewModels/SuperAdminDashboardViewModel.cs
    public class SuperAdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int SuperAdminCount { get; set; }
        public int AdminCount { get; set; }
        public int UserCount { get; set; }

        public int NewThisMonth { get; set; }          // Registered this month
        public int LockedAccounts { get; set; }        // If using Identity lockout

        // Optional: for future extension
        public DateTime? LastUserRegistered { get; set; }
    }
}
