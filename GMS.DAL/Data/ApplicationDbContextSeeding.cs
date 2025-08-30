using GMS.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace GMS.DAL.Data;
public static class ApplicationDbContextSeeding
{
	public static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
	{
		string[] roleNames = ["SuperAdmin", "Admin", "Instructor", "Trainee"];

		foreach (var roleName in roleNames)
		{
			if (!await roleManager.RoleExistsAsync(roleName))
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}
	}

	public static async Task CreateDefaultSuperAdminAsync(UserManager<ApplicationUser> userManager)
	{
		const string superAdminEmail = "superadmin@Gradems.com";
		const string superAdminPassword = "SuperAdmin123!";

		var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
		if (superAdmin == null)
		{
			superAdmin = new ApplicationUser
			{
				UserName = superAdminEmail,
				Email = superAdminEmail,
				FullName = "System Super Administrator",
				Role = UserRole.SuperAdmin,
				EmailConfirmed = true,
				IsActive = true
			};

			var result = await userManager.CreateAsync(superAdmin, superAdminPassword);
			if (result.Succeeded) await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
		}
	}
}