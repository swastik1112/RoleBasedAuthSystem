namespace RoleBasedAuthSystem.Models.ViewModels
{
    // Models/ViewModels/UserListViewModel.cs
    public class UserListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();

        public string SearchTerm { get; set; } = string.Empty;

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalUsers { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalUsers / PageSize);

        // For Admin - to know if we filtered to only Users
        public bool IsAdminView { get; set; }
    }
}
