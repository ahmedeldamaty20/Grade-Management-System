using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMS.DAL.Models;
using GMS.DAL.Interfaces;
using GMS.BLL.Interfaces;

namespace GMS.BLL.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepo _courseRepo;

        public CourseService(ICourseRepo courseRepo)
        {
            _courseRepo = courseRepo;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync(string? search = null, string? category = null, int pageNumber = 1, int pageSize = 10)
        {
            return await _courseRepo.GetAllAsync(search,category, pageNumber, pageSize);
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            var addedCourse = await _courseRepo.AddAsync(course);
            return addedCourse;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var result = await _courseRepo.DeleteAsync(id);
            return result;
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _courseRepo.GetByIdAsync(id);
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            var result = await _courseRepo.UpdateAsync(course);
            return result;
        }


        public async Task SaveChangesAsync() { 
        
        await _courseRepo.SaveChangesAsync();
        }
        public async Task<int> GetCoursesCountAsync(string? search, string? category)
        {
            int totalCount = await _courseRepo.GetCoursesCountAsync(search, category);
            return totalCount;
        }
    }
}
