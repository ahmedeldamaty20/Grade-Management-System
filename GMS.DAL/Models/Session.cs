namespace GMS.DAL.Models;
public class Session
{
	public int Id { get; set; }
	public int CourseId { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public virtual Course Course { get; set; } = null!;
	public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}