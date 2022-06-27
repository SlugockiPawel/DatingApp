﻿using DatingApp.DTOs;
using DatingApp.Models;

namespace DatingApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser> GetUserByIdAsync(Guid id);
        Task<AppUser> GetUserByNameAsync(string name);
        Task<UserDto> RegisterUserAsync(RegisterDto registerDto);
        Task<bool> UserExistAsync(string username);
    }
}
