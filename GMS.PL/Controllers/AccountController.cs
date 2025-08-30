using GMS.DAL.Data;
using GMS.DAL.Models;
using GMS.DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GMS.DAL.Controllers;
public class AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager,
ILogger<AccountController> _logger, ApplicationDbContext _dbContext) : Controller
{

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in successfully", model.Email);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account locked out", model.Email);
            ModelState.AddModelError(string.Empty, "Your account has been locked out due to multiple failed login attempts.");
        }
        else
        {
            _logger.LogWarning("Invalid login attempt for {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
        }

        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login");
        }

        if (model.Role == UserRole.Admin && currentUser.Role != UserRole.SuperAdmin)
        {
            ModelState.AddModelError(string.Empty, "Only Super Administrators can create Admin accounts.");
            return View(model);
        }

        if (model.Role == UserRole.SuperAdmin && currentUser.Role != UserRole.SuperAdmin)
        {
            ModelState.AddModelError(string.Empty, "Only Super Administrators can create Super Admin accounts.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Role = model.Role,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} created successfully with role {Role}", model.Email, model.Role);

            await _userManager.AddToRoleAsync(user, model.Role.ToString());

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User {model.FullName} has been created successfully.";
            return RedirectToAction("ManageUsers");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterStudent()
    {
        return View(new RegisterViewModel { Role = UserRole.Trainee });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterStudent(RegisterViewModel model)
    {
        model.Role = UserRole.Trainee;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Role = UserRole.Trainee,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("Student {Email} registered successfully", model.Email);

            await _userManager.AddToRoleAsync(user, UserRole.Trainee.ToString());

            await _dbContext.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["SuccessMessage"] = $"Welcome {model.FullName}! Your student account has been created successfully.";
            return RedirectToAction("Dashboard", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }


    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> ManageUsers(string? searchTerm, UserRole? roleFilter, bool? isActiveFilter)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var users = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            users = users.Where(u => u.FullName.Contains(searchTerm) || u.Email!.Contains(searchTerm));
        }

        if (roleFilter.HasValue)
        {
            users = users.Where(u => u.Role == roleFilter.Value);
        }

        if (isActiveFilter.HasValue)
        {
            users = users.Where(u => u.IsActive == isActiveFilter.Value);
        }

        if (currentUser?.Role != UserRole.SuperAdmin)
        {
            users = users.Where(u => u.Role != UserRole.SuperAdmin);
        }

        var userList = users.Select(u => new UserProfileViewModel
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email!,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();

        var viewModel = new ManageUsersViewModel
        {
            Users = userList,
            SearchTerm = searchTerm,
            RoleFilter = roleFilter,
            IsActiveFilter = isActiveFilter
        };

        return View(viewModel);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserStatus(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var targetUser = await _userManager.FindByIdAsync(userId);

        if (targetUser == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("ManageUsers");
        }

        if (targetUser.Role == UserRole.SuperAdmin && currentUser?.Role != UserRole.SuperAdmin)
        {
            TempData["ErrorMessage"] = "You don't have permission to modify Super Administrator accounts.";
            return RedirectToAction("ManageUsers");
        }

        if (targetUser.Id == currentUser?.Id)
        {
            TempData["ErrorMessage"] = "You cannot deactivate your own account.";
            return RedirectToAction("ManageUsers");
        }

        targetUser.IsActive = !targetUser.IsActive;
        targetUser.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(targetUser);

        if (result.Succeeded)
        {
            var status = targetUser.IsActive ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"User {targetUser.FullName} has been {status}.";
            _logger.LogInformation("User {UserId} {Status} by {CurrentUserId}",  targetUser.Id, status, currentUser?.Id);
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update user status.";
        }

        return RedirectToAction("ManageUsers");
    }


    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new UserProfileViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return View(model);
    }


    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your password has been changed successfully.";
            _logger.LogInformation("User {UserId} changed their password", user.Id);
            return RedirectToAction("Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UserDetails(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var targetUser = await _userManager.FindByIdAsync(id);
        if (targetUser == null)
        {
            return NotFound();
        }

        if (targetUser.Role == UserRole.SuperAdmin && currentUser?.Role != UserRole.SuperAdmin)
        {
            return Forbid();
        }

        var model = new UserProfileViewModel
        {
            Id = targetUser.Id,
            FullName = targetUser.FullName,
            Email = targetUser.Email!,
            Role = targetUser.Role,
            IsActive = targetUser.IsActive,
            CreatedAt = targetUser.CreatedAt,
            UpdatedAt = targetUser.UpdatedAt
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
	    await _signInManager.SignOutAsync();
	    _logger.LogInformation("User logged out");
	    return RedirectToAction("Index", "Home");
    }
}
