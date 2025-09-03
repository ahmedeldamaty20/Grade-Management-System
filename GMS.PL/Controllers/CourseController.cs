using GMS.BLL.Interfaces;
using GMS.BLL.ViewModels;
using GMS.DAL.Data;
using GMS.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GMS.PL.Controllers;
public class CourseController(ICourseService courseService, UserManager<ApplicationUser> userManager, ApplicationDbContext context) : Controller
{
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> Index(string? search, string? category, int pageNumber = 1, int pageSize = 10)
	{
		if (category == "All")
		{
			category = null;
		}
		var courses = await courseService.GetAllCoursesAsync(search, category, pageNumber, pageSize);
		var totalCourses = await courseService.GetCoursesCountAsync(search, category);

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
		if (model.Category == 0)
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

		var changes = await courseService.AddCourseAsync(course);

		if(changes is not null) await courseService.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	[Authorize(Roles = "Admin,SuperAdmin")]
	public async Task<IActionResult> Edit(int id)
	{
		var course = await courseService.GetCourseByIdAsync(id);
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

		if (model.Category == 0)
		{
			ModelState.AddModelError("Category", "Category is required");
			await PopulateInstructorsDropDown(model.InstructorId);
			return View(model);
		}

		var course = await courseService.GetCourseByIdAsync(model.Id);
		if (course == null)
			return NotFound();

		course.Name = model.Name;
		course.Category = model.Category.ToString();
		course.InstructorId = model.InstructorId;

		await courseService.UpdateCourseAsync(course);
		await courseService.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "Admin,SuperAdmin")]

	public async Task<IActionResult> Delete(int id)
	{
		var course = await courseService.GetCourseByIdAsync(id);
		if (course == null)
			return NotFound();
		await courseService.DeleteCourseAsync(id);
		await courseService.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
	private async Task PopulateInstructorsDropDown(string selectedValue)
	{
		var instructors = await userManager.GetUsersInRoleAsync("Instructor");
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
		bool exists = context.Course.Any(c => c.Name == name);
		if (exists)
			return Json($"Course name '{name}' is already taken.");
		return Json(true);
	}
}
