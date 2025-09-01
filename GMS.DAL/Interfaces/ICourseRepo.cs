using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMS.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GMS.DAL.Interfaces
{
    public interface ICourseRepo
    {
        // ✅ Get all with optional search + pagination
        Task<IEnumerable<Course>> GetAllAsync(string? search = null,string?category=null, int pageNumber = 1, int pageSize = 10);

        Task<Course?> GetByIdAsync(int id);
        Task<Course> AddAsync(Course course);
        Task<bool> UpdateAsync(Course course);
        Task<bool> DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<int> GetCoursesCountAsync(string? search, string? category);

    }
}
