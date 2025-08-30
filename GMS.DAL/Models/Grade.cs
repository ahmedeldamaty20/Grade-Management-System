namespace GMS.DAL.Models;
public class Grade
{
	public int Id { get; set; }
	public int SessionId { get; set; }
	public string TraineeId { get; set; } = string.Empty;
	public int Value { get; set; }
	public virtual Session Session { get; set; } = null!;
	public virtual ApplicationUser Trainee { get; set; } = null!;
}
