using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await _userService.UserExistAsync(registerDto.Name))
            {
                return BadRequest("Username is taken");
            }

            return await _userService.RegisterUserAsync(registerDto);
        }
    }
}
