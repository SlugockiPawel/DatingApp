﻿using System.Security.Cryptography;
using System.Text;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services
{
    public class UserServicePostgres : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;


        public UserServicePostgres(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Id.Equals(id));
        }

        public async Task<AppUser> GetUserByNameAsync(string name)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Name.Equals(name));
        }

        public async Task<UserDto> RegisterUserAsync(RegisterDto registerDto)
        {
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                Name = registerDto.Name.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto()
            {
                Name = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }
        public async Task<bool> UserExistAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Name.Equals(username.ToLower()));
        }
    }
}