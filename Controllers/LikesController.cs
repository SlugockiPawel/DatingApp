﻿using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

[Authorize]
public sealed class LikesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public LikesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var likedUser = await _unitOfWork.UserService.GetUserByNameAsync(username);
        var sourceUser = await _unitOfWork.LikeService.GetUserWithLikes(sourceUserId);

        if (likedUser is null)
        {
            return NotFound();
        }

        if (sourceUser.UserName.Equals(username))
        {
            return BadRequest("You cannot give yourself a like.");
        }

        var userLike = await _unitOfWork.LikeService.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike is not null)
        {
            return BadRequest("You already like this user");
        }

        userLike = new AppUserLike { SourceUserId = sourceUserId, LikedUserId = likedUser.Id };

        sourceUser.LikedUsers.Add(userLike);

        if (await _unitOfWork.Complete())
        {
            return Ok();
        }

        return BadRequest("Failed to like user");
    }

    [HttpDelete("{username}")]
    public async Task<ActionResult> RemoveLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var dislikedUser = await _unitOfWork.UserService.GetUserByNameAsync(username);
        var loggedUser = await _unitOfWork.LikeService.GetUserWithLikes(sourceUserId);

        if (dislikedUser is null)
        {
            return NotFound();
        }

        if (loggedUser.UserName.Equals(username))
        {
            return BadRequest("You cannot dislike yourself");
        }

        var userLike = await _unitOfWork.LikeService.GetUserLike(sourceUserId, dislikedUser.Id);

        if (userLike is null)
        {
            return BadRequest("You already disliked this user");
        }

        _unitOfWork.LikeService.DeleteLike(userLike);

        if (await _unitOfWork.Complete())
        {
            return NoContent();
        }

        return BadRequest("Failed to dislike the user");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(
        [FromQuery] LikesParams likesParams
    )
    {
        likesParams.UserId = User.GetUserId();
        var users = await _unitOfWork.LikeService.GetUserLikes(likesParams);

        Response.AddPaginationHeader(
            users.CurrentPage,
            users.PageSize,
            users.TotalCount,
            users.TotalPages
        );

        return Ok(users);
    }
}