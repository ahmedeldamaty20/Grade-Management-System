using GMS.DAL.Models;

namespace GMS.BLL.ViewModels;
public class TraineeGradesViewModel
{
    public ApplicationUser Trainee { get; set; } = null!;
    public List<Grade> Grades { get; set; } = new List<Grade>();

}