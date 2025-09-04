using GMS.BLL.Interfaces;
using GMS.DAL.Interfaces;
using GMS.DAL.Models;
using GMS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS.BLL.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepo _sessionRepo;

        public SessionService(ISessionRepo sessionRepo)
        {
            _sessionRepo = sessionRepo;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsAsync(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            return await _sessionRepo.GetAllAsync(search, pageNumber, pageSize);
        }

        public async Task<Session> AddSessionAsync(Session session)
        {
            var addedSession = await _sessionRepo.AddAsync(session);
            return addedSession;
        }

        public async Task<bool> DeleteSessionAsync(int id)
        {
            var result = await _sessionRepo.DeleteAsync(id);
            return result;
        }

        public async Task<Session?> GetSessionByIdAsync(int id)
        {
            return await _sessionRepo.GetByIdAsync(id);
        }

        public async Task<bool> UpdateSessionAsync(Session session)
        {
            var result = await _sessionRepo.UpdateAsync(session);
            return result;
        }


        public async Task SaveChangesAsync()
        {
            await _sessionRepo.SaveChangesAsync();
        }
        public async Task<int> GetSessionsCountAsync(string? search)
        {
            int totalCount = await _sessionRepo.GetSessionsCountAsync(search);
            return totalCount;
        }
    }
}
