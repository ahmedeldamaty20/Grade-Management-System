using GMS.DAL.Data;
using GMS.DAL.Interfaces;
using GMS.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GMS.DAL.Repositories;
public class AccountRepository(ApplicationDbContext context) : Repository<ApplicationUser>(context), IAccountRepository
{
    public async Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(UserRole role)
    {
        return await _dbSet
            .Where(u => u.Role == role)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email!.ToLower() == email.ToLower());

        if (!string.IsNullOrEmpty(excludeId))
        {
            query = query.Where(u => u.Id != excludeId);
        }

        return !await query.AnyAsync();
    }

    public async Task<ApplicationUser?> GetByStringIdAsync(string id)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
    }
}