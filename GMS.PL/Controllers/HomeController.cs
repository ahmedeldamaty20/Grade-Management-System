using GMS.BLL.Interfaces;
using GMS.DAL.Models;
using GMS.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GMS.PL.Controllers;
public class HomeController(UserManager<ApplicationUser> userManager,ICourseService _courseService) : Controller
{

    public IActionResult Index()
	{
		if (User.Identity?.IsAuthenticated == true)
		{
			return RedirectToAction("Dashboard");
		}

		return View("Landing");
	}

	[Authorize]
	public async Task<IActionResult> Dashboard()
	{
		var currentUser = await userManager.GetUserAsync(User);

		ViewBag.TotalCourses = (await _courseService.GetCoursesCountAsync(null,null));
		//ViewBag.TotalSessions = (await _sessionRepository.GetAllAsync()).Count();
		//ViewBag.TotalUsers = (await _userRepository.GetAllAsync()).Count();
		//ViewBag.TotalGrades = (await _gradeRepository.GetAllAsync()).Count();

		ViewBag.TotalSessions = 0;
		ViewBag.TotalUsers = 0;
		ViewBag.TotalGrades = 0;

		ViewBag.UserRole = currentUser?.Role.ToString();
		ViewBag.UserName = currentUser?.FullName;

		return View();
	}
}
