using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GMS.DAL.Data;
using GMS.DAL.Interfaces;
using GMS.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GMS.DAL.Repositories
{
    public class SessionRepo : ISessionRepo
    {
        private readonly ApplicationDbContext _context;

        public SessionRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Session> AddAsync(Session entity)
        {
            await _context.Session.AddAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var session = await GetByIdAsync(id);
            if (session == null)
                return false;

            _context.Session.Remove(session);
            return true;
        }

        public async Task<IEnumerable<Session>> GetAllAsync(string? search = null,int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Session
                .Include(c => c.Course)
                .Include(c => c.Grades)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Course.Name.Contains(search));
            }
            

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Session?> GetByIdAsync(int id)
        {
            return await _context.Session
                .Include(c => c.Course)
                .Include(c => c.Grades)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateAsync(Session session)
        {


            _context.Session.Update(session);
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetSessionsCountAsync(string? search)
        {
            var query = _context.Session.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Course.Name.Contains(search));

            
            int total= await query.CountAsync();
            return total;
        }
    }
}
