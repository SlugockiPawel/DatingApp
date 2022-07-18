using DatingApp.Extensions;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
            return;

        var username = resultContext.HttpContext.User.GetUserName();
        var repo = resultContext.HttpContext.RequestServices.GetService<IUserService>();
        var user = await repo.GetUserByNameAsync(username);
        user.LastActive = DateTime.Now;
        await repo.SaveAllAsync();
    }
}