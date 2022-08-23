using DatingApp.Enums;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers;

public class AdminsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public AdminsController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
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
    public async Task<ActionResult> GetPhotosForModeration()
    {
        var photos = await _unitOfWork.PhotoService.GetUnapprovedPhotosAsync();

        return Ok(photos);
    }

    [Authorize(Policy = $"{nameof(AuthPolicies.ModeratePhotoRole)}")]
    [HttpPut("approve-photo/{photoId:int}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _unitOfWork.PhotoService.GetPhotoByIdAsync(photoId);

        if (photo is null)
        {
            return BadRequest("Cannot approve the photo");
        }

        photo.IsApproved = true;

        var user = await _unitOfWork.UserService.GetUserByPhotoIdAsync(photoId);
        if (!_unitOfWork.UserService.HasMainPhoto(user))
        {
            photo.IsMain = true;
        }

        if (await _unitOfWork.Complete())
        {
            return NoContent();
        }

        return BadRequest("Cannot approve the photo");
    }

    [Authorize(Policy = $"{nameof(AuthPolicies.ModeratePhotoRole)}")]
    [HttpDelete("reject-photo/{photoId:int}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await _unitOfWork.PhotoService.GetPhotoByIdAsync(photoId);

        if (photo.PublicId is not null)
        {
            await _unitOfWork.PhotoService.DeletePhotoAsync(photo.PublicId);
        }

        _unitOfWork.PhotoService.RemovePhoto(photo);
        await _unitOfWork.Complete();
        return NoContent();
    }
}