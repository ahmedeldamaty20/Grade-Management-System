using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMS.DAL.Models;

namespace GMS.BLL.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync(string? search = null, string?category=null,int pageNumber = 1, int pageSize = 10);
        Task<Course> AddCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);
        Task<Course?> GetCourseByIdAsync(int id);
        Task<bool> UpdateCourseAsync(Course course);
        //Task<bool> ExistsByNameAsync(string name);
        Task SaveChangesAsync();

        Task<int> GetCoursesCountAsync(string? search, string? category);

    }
}
