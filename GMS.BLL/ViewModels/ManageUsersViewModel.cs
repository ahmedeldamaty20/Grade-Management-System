using GMS.DAL.Models;

namespace GMS.DAL.ViewModels;
public class ManageUsersViewModel
{
    public List<UserProfileViewModel> Users { get; set; } = new List<UserProfileViewModel>();
    public string? SearchTerm { get; set; }
    public UserRole? RoleFilter { get; set; }
    public bool? IsActiveFilter { get; set; }
}
