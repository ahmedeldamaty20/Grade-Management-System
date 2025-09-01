namespace GMS.DAL.Models;
public enum CourseCategory
{
    //None = 0,
    SoftwareDevelopment=1,
    WebDevelopment,
    Marketing,
    Business,
    Design
}
public class Course
{
//test
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Category { get; set; }
	public string? InstructorId { get; set; }
	public virtual ApplicationUser? Instructor { get; set; }
	public virtual ICollection<Session>? Sessions { get; set; } = new List<Session>();
}