using GMS.DAL.Interfaces;
using GMS.DAL.Models;

namespace GMS.BLL.Interfaces;
public interface IGradeRepository : IRepository<Grade>
{
    Task<IEnumerable<Grade>> GetGradesByTraineeAsync(string traineeId);
    Task<IEnumerable<Grade>> GetGradesBySessionAsync(int sessionId);
    Task<Grade?> GetGradeBySessionAndTraineeAsync(int sessionId, string traineeId);
    Task<IEnumerable<Grade>> GetGradesWithDetailsAsync();
}