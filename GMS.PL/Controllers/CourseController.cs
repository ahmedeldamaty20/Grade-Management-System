using GMS.DAL.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GMS.BLL.Interfaces;
using GMS.BLL.ViewModels;
using Microsoft.EntityFrameworkCore;
using GMS.DAL.Data;
using Microsoft.AspNetCore.Authorization;
namespace GMS.PL.Controllers
{

    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public CourseController(ICourseService courseService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _courseService = courseService;
            _userManager = userManager;
            _context = context; 
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, string? category, int pageNumber = 1, int pageSize = 10)
        {
            if (category == "All")
            {
                category = null;
            }
            var courses = await _courseService.GetAllCoursesAsync(search, category, pageNumber, pageSize);
            int totalCourses = await _courseService.GetCoursesCountAsync(search, category);

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCourses / pageSize);
            await PoputlatePageSize(pageSize);
            var items = Enum.GetValues(typeof(CourseCategory))
                .Cast<CourseCategory>()
                .Select(e => new
                {
                    Id = e.ToString(),
                    Name = e.ToString()
                });

            ViewBag.Categories = new SelectList(items, "Id", "Name", category);


            return View(courses);
        }


        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            await PopulateInstructorsDropDown(null);
           

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(CourseVM model)
        {
            if (model.Category ==0)
                ModelState.AddModelError("Category", "Category is required");

            if (!ModelState.IsValid)
            {
                await PopulateInstructorsDropDown(model.InstructorId);
                return View(model);
            }

            var course = new Course
            {
                Name = model.Name,
                Category = model.Category.ToString(),
                InstructorId = model.InstructorId
            };

            await _courseService.AddCourseAsync(course);
            await _courseService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int id)
        {

            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            var model = new CourseVM
            {
                Name = course.Name,
                Category = (CourseCategory)Enum.Parse(typeof(CourseCategory), course.Category),
                InstructorId = course.InstructorId
            };
            await PopulateInstructorsDropDown(model.InstructorId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateInstructorsDropDown(model.InstructorId);

                return View(model);
            }

            if (model.Category==0)
            {
                ModelState.AddModelError("Category", "Category is required");
                await PopulateInstructorsDropDown(model.InstructorId);
                return View(model);
            }

            var course = await _courseService.GetCourseByIdAsync(model.Id);
            if (course == null)
                return NotFound();

            course.Name = model.Name;
            course.Category = model.Category.ToString();
            course.InstructorId = model.InstructorId;

            await _courseService.UpdateCourseAsync(course);
            await _courseService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();
            await _courseService.DeleteCourseAsync(id);
            await _courseService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateInstructorsDropDown(string selectedValue)
        {
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
            ViewBag.Instructors = new SelectList(instructors, "Id", "FullName", selectedValue);
        }

        private async Task PopulateCategoryDropDown(int selectedValue)
        {
            var items = Enum.GetValues(typeof(CourseCategory))
                            .Cast<CourseCategory>()
                            .Select(e => new
                            {
                                Id = (int)e,
                                Name = e.ToString()
                            });

            ViewBag.Categories = new SelectList(items, "Id", "Name", selectedValue);

        }
        private async Task PoputlatePageSize(int pageSize)
        {
            var pageSizes = new List<int> { 5, 10, 15 };
            ViewBag.PageSizes = new SelectList(pageSizes, pageSize);
        }
        public IActionResult NameExistVal(string name)
        {
            bool exists = _context.Course.Any(c=>c.Name==name);
            if (exists)
                return Json($"Course name '{name}' is already taken.");
            return Json(true);
        }
    }
}
