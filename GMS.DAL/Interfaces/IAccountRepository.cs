using GMS.DAL.Models;

namespace GMS.DAL.Interfaces;
public interface IAccountRepository
{
    Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(UserRole role);
    Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null);
    Task<ApplicationUser?> GetByStringIdAsync(string id);
}