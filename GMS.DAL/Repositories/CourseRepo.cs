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
    public class CourseRepo : ICourseRepo
    {
        private readonly ApplicationDbContext _context;

        public CourseRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Course> AddAsync(Course entity)
        {
            await _context.Course.AddAsync(entity);
            //await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await GetByIdAsync(id);
            if (course == null)
                return false;

            _context.Course.Remove(course);
            return true;
        }

        public async Task<IEnumerable<Course>> GetAllAsync(string? search = null,string?category=null ,int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Course
                .Include(c => c.Instructor)
                .Include(c => c.Sessions)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search));
            }
            if (category != null)
            {
                query = query.Where(c => c.Category == category);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Course
                .Include(c => c.Instructor)
                .Include(c => c.Sessions)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateAsync(Course course)
        {


            _context.Course.Update(course);
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetCoursesCountAsync(string? search, string? category)
        {
            var query = _context.Course.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(c => c.Category == category);

            int total= await query.CountAsync();
            return total;
        }
    }
}
