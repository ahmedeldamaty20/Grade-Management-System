using GMS.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace GMS.DAL.ViewModels;
public class UserProfileViewModel
{
	public string Id { get; set; } = string.Empty;

	[Required(ErrorMessage = "Full name is required")]
	[StringLength(50, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 50 characters")]
	[Display(Name = "Full Name")]
	public string FullName { get; set; } = string.Empty;

	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Please enter a valid email address")]
	public string Email { get; set; } = string.Empty;

	public UserRole Role { get; set; }

	[Display(Name = "Active")]
	public bool IsActive { get; set; }

	[Display(Name = "Created At")]
	public DateTime CreatedAt { get; set; }

	[Display(Name = "Last Updated")]
	public DateTime? UpdatedAt { get; set; }
}