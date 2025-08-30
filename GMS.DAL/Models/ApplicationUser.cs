using Microsoft.AspNetCore.Identity;

namespace GMS.DAL.Models;
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Trainee;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<Course> CoursesAsInstructor { get; set; } = new List<Course>();
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}