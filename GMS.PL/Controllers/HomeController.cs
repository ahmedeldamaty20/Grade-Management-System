using GMS.BLL.Interfaces;
using GMS.DAL.Interfaces;
using GMS.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GMS.PL.Controllers;
public class HomeController(UserManager<ApplicationUser> userManager,ICourseService _courseService, ISessionService _sessionService, IAccountRepository _accountRepository, IGradeRepository _gradeRepository) : Controller
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

		ViewBag.TotalCourses = await _courseService.GetCoursesCountAsync(null,null);
		ViewBag.TotalSessions = await _sessionService.GetSessionsCountAsync(null);
		ViewBag.TotalUsers = (await _accountRepository.GetAllAsync()).Count();
		ViewBag.TotalGrades = (await _gradeRepository.GetAllAsync()).Count();

		ViewBag.UserRole = currentUser?.Role.ToString();
		ViewBag.UserName = currentUser?.FullName;

		return View();
	}
}
