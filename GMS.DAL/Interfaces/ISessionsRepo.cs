using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMS.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GMS.DAL.Interfaces
{
    public interface ISessionRepo
    {
        Task<IEnumerable<Session>> GetAllAsync(string? search = null, int pageNumber = 1, int pageSize = 10);

        Task<Session?> GetByIdAsync(int id);
        Task<Session> AddAsync(Session course);
        Task<bool> UpdateAsync(Session course);
        Task<bool> DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<int> GetSessionsCountAsync(string? search);

    }
}
