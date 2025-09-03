using GMS.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace GMS.DAL.ViewModels;
public class GradeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Session is required")]
    [Display(Name = "Session")]
    public int SessionId { get; set; }

    [Required(ErrorMessage = "Trainee is required")]
    [Display(Name = "Trainee")]
    public string TraineeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Grade value is required")]
    [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100")]
    [Display(Name = "Grade Value")]
    public int Value { get; set; }

    public string? SessionInfo { get; set; }
    public string? TraineeName { get; set; }
    public List<Session> AvailableSessions { get; set; } = new List<Session>();
    public List<ApplicationUser> AvailableTrainees { get; set; } = new List<ApplicationUser>();
}