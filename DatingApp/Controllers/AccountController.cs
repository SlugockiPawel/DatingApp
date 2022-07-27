using DatingApp.DTOs;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AccountController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }


    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await _userService.UserExistAsync(registerDto.UserName)) return BadRequest("Username is taken");

        return await _userService.RegisterUserAsync(registerDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userService.GetUserByNameAsync(loginDto.UserName);

        if (user is null) return Unauthorized("Invalid username");

        return new UserDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user),
            MainPhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }
}