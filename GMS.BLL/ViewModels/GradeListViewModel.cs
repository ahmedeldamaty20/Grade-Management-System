using GMS.DAL.Models;

namespace GMS.DAL.ViewModels;
public class GradeListViewModel
{
    public List<Grade> Grades { get; set; } = new List<Grade>();
    public string? FilterByTraineeId { get; set; }
    public int? FilterBySessionId { get; set; }
    public List<ApplicationUser> AvailableTrainees { get; set; } = new List<ApplicationUser>();
    public List<Session> AvailableSessions { get; set; } = new List<Session>();
}