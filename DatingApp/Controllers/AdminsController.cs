using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

public class AdminsController : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public IActionResult GetUsersWithRoles()
    {
        return Ok("Only admins can see this");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public IActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}