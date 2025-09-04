using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMS.DAL.Models;

namespace GMS.BLL.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<Session>> GetAllSessionsAsync(string? search = null,int pageNumber = 1, int pageSize = 10);
        Task<Session> AddSessionAsync(Session session);
        Task<bool> DeleteSessionAsync(int id);
        Task<Session?> GetSessionByIdAsync(int id);
        Task<bool> UpdateSessionAsync(Session session);
        //Task<bool> ExistsByNameAsync(string name);
        Task SaveChangesAsync();

        Task<int> GetSessionsCountAsync(string? search);

    }
}
