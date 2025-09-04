using GMS.BLL.Interfaces;
using GMS.DAL.Data;
using GMS.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GMS.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ICourseService _courseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SessionsController(
            ISessionService sessionService,
            ICourseService courseService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _sessionService = sessionService;
            _courseService = courseService;
            _userManager = userManager;
            _context = context;
        }

        // ------------------- Index -------------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Session
            .Include(s => s.Course)
            .ThenInclude(c => c.Instructor) // course instructor
            .Include(s => s.Grades)            // session grades
            .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Course.Name.Contains(search));
            }
            int totalSessions = await _sessionService.GetSessionsCountAsync(search);
            ViewBag.Search = search; ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize; ViewBag.TotalPages = (int)Math.Ceiling((double)totalSessions / pageSize);
            await PopulatePageSize(pageSize);
            IEnumerable<Session> sessions = query.AsEnumerable<Session>();
            return View(sessions.OrderBy(s => s.StartDate)  // or any property you prefer
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize));
        }

        // ------------------- Create -------------------
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            await PopulateCoursesDropDown(null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(Session session)
        {
            // Validation
            if (session.CourseId == 0)
                ModelState.AddModelError("CourseId", "Course is required");

            if (session.StartDate == default)
                ModelState.AddModelError("StartDate", "Start date is required");
            else if (session.StartDate.Date < DateTime.Today)
                ModelState.AddModelError("StartDate", "Start date cannot be in the past");

            if (session.EndDate == default)
                ModelState.AddModelError("EndDate", "End date is required");
            else if (session.EndDate <= session.StartDate)
                ModelState.AddModelError("EndDate", "End date must be after start date");

            if (!ModelState.IsValid)
            {
                await PopulateCoursesDropDown(session.CourseId);
                return View(session);
            }

            await _sessionService.AddSessionAsync(session);
            await _sessionService.SaveChangesAsync();

            TempData["SuccessMessage"] = "Session created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ------------------- Edit -------------------
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _sessionService.GetSessionByIdAsync(id);
            if (session == null)
                return NotFound();

            await PopulateCoursesDropDown(session.CourseId);
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(Session session)
        {
            // Same validation as Create
            if (session.CourseId == 0)
                ModelState.AddModelError("CourseId", "Course is required");

            if (session.StartDate == default)
                ModelState.AddModelError("StartDate", "Start date is required");
            else if (session.StartDate.Date < DateTime.Today)
                ModelState.AddModelError("StartDate", "Start date cannot be in the past");

            if (session.EndDate == default)
                ModelState.AddModelError("EndDate", "End date is required");
            else if (session.EndDate <= session.StartDate)
                ModelState.AddModelError("EndDate", "End date must be after start date");

            if (!ModelState.IsValid)
            {
                await PopulateCoursesDropDown(session.CourseId);
                return View(session);
            }

            await _sessionService.UpdateSessionAsync(session);
            await _sessionService.SaveChangesAsync();

            TempData["SuccessMessage"] = "Session updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ------------------- Delete -------------------
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _sessionService.GetSessionByIdAsync(id);
            if (session == null)
                return NotFound();

            await _sessionService.DeleteSessionAsync(id);
            await _sessionService.SaveChangesAsync();
            TempData["SuccessMessage"] = "Session deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ------------------- Helpers -------------------
        private async Task PopulateInstructorsDropDown(string? selectedValue)
        {
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
            ViewBag.Instructors = new SelectList(instructors, "Id", "FullName", selectedValue);
        }

        private async Task PopulateCoursesDropDown(int? selectedValue)
        {
            var courses = await _courseService.GetAllCoursesAsync();
            ViewBag.Courses = new SelectList(courses, "Id", "Name", selectedValue);
        }

        private async Task PopulatePageSize(int pageSize)
        {
            var pageSizes = new List<int> { 5, 10, 15 };
            ViewBag.PageSizes = new SelectList(pageSizes, pageSize);
        }
    }
}
