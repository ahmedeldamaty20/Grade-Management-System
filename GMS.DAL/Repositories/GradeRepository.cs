using GMS.BLL.Interfaces;
using GMS.DAL.Data;
using GMS.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GMS.DAL.Repositories;
public class GradeRepository(ApplicationDbContext context) : Repository<Grade>(context), IGradeRepository
{
    public async Task<Grade?> GetGradeBySessionAndTraineeAsync(int sessionId, string traineeId)
    {
        return await _dbSet
            .Include(g => g.Session)
            .ThenInclude(s => s.Course)
            .Include(g => g.Trainee)
            .FirstOrDefaultAsync(g => g.SessionId == sessionId && g.TraineeId == traineeId);
    }

    public async Task<IEnumerable<Grade>> GetGradesBySessionAsync(int sessionId)
    {
        return await _dbSet
            .Include(g => g.Session)
            .ThenInclude(s => s.Course)
            .Include(g => g.Trainee)
            .Where(g => g.SessionId == sessionId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Grade>> GetGradesByTraineeAsync(string traineeId)
    {
        return await _dbSet
            .Include(g => g.Session)
            .ThenInclude(s => s.Course)
            .Include(g => g.Trainee)
            .Where(g => g.TraineeId == traineeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Grade>> GetGradesWithDetailsAsync()
    {
        return await _dbSet
            .Include(g => g.Session)
            .ThenInclude(s => s.Course)
            .Include(g => g.Trainee)
            .ToListAsync();
    }

    public override async Task<Grade?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(g => g.Session)
            .ThenInclude(s => s.Course)
            .Include(g => g.Trainee)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}