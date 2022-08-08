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
    public IActionResult GetUsersWithRoles()
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

    [HttpPost("edit-roles/{username}")]
    [Authorize(Policy = $"{nameof(AuthPolicies.ModeratePhotoRole)}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        var selectedRoles = roles.Split(",");
        var user = await _userManager.FindByNameAsync(username);

        if (user is null)
            return NotFound("User not found");

        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded)
            return BadRequest("Failed to add roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded)
            return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = $"{nameof(AuthPolicies.ModeratePhotoRole)}")]
    [HttpGet("photos-to-moderate")]
    public IActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}