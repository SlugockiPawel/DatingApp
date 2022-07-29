using DatingApp.Enums;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers;

public class AdminsController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;

    public AdminsController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Policy = $"{nameof(AuthPolicies.RequireAdminRole)}")]
    [HttpGet("users-with-roles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = _userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.UserName)
            .Select(
                u =>
                    new
                    {
                        u.Id,
                        u.UserName,
                        Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                    }
            );

        return Ok(users);
    }

    [Authorize(Policy = $"{nameof(AuthPolicies.ModeratePhotoRole)}")]
    [HttpGet("photos-to-moderate")]
    public IActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}