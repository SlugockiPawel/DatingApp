﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services;

public sealed class UserServicePostgres : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public UserServicePostgres(
        ApplicationDbContext context,
        ITokenService tokenService,
        IMapper mapper
    )
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
    {
        return await _context.Users.Include(u => u.Photos).ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(Guid id)
    {
        // return await _context.Users.FindAsync(id);
        return await _context.Users.IgnoreQueryFilters().Include(u => u.Photos).SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<AppUser> GetUserByNameAsync(string name)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Photos)
            .Include(u =>u.LikedUsers)
            .SingleOrDefaultAsync(u => u.UserName.Equals(name));
    }

    public async Task<UserDto> RegisterUserAsync(RegisterDto registerDto)
    {
        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.UserName.ToLower();

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateTokenAsync(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> UserExistAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.UserName.Equals(username.ToLower()));
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

        var query = _context.Users
            .AsQueryable()
            .Where(u => u.Gender == userParams.Gender && u.UserName != userParams.CurrentUserName)
            .Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth <= maxDateOfBirth)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .AsNoTracking();

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };

        return await PagedList<MemberDto>.CreateAsync(
            query,
            userParams.PageNumber,
            userParams.PageSize
        );
    }

    public async Task<MemberDto> GetMemberByNameAsync(string name, bool isCurrentUser)
    {
        // do not need to .Include(u=> u.Photo), mapper and EF will perform left join
        var query = _context.Users
            .Where(u => u.UserName == name)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .AsQueryable();

        if (isCurrentUser)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<string> GetUserGender(string username)
    {
        return await _context.Users
            .Where(u => u.UserName == username)
            .Select(u => u.Gender)
            .FirstOrDefaultAsync();
    }

    public async Task<AppUser> GetUserByPhotoIdAsync(int photoId)
    {
        try
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Photos.Any(p => p.Id == photoId));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool HasMainPhoto(AppUser user)
    {
        try
        {
            return user.Photos.Any(p => p.IsMain);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}