﻿using DatingApp.Extensions;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.Helpers;

public sealed class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
            return;

        var userId = resultContext.HttpContext.User.GetUserId();
        var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
        var user = await unitOfWork.UserService.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;
        await unitOfWork.Complete();
    }
}