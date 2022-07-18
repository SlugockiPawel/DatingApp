using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services;

public class UserServicePostgres : IUserService
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
        return await _context.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => u.Id.Equals(id));
    }

    public async Task<AppUser> GetUserByNameAsync(string name)
    {
        return await _context.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => u.Name.Equals(name));
    }

    public async Task<UserDto> RegisterUserAsync(RegisterDto registerDto)
    {
        var user = _mapper.Map<AppUser>(registerDto);

        using var hmac = new HMACSHA512();

        user.Name = registerDto.Name.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Name = user.Name,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UserExistAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Name.Equals(username.ToLower()));
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

        var query = _context.Users
            .AsQueryable()
            .Where(u => u.Gender == userParams.Gender && u.Name != userParams.CurrentUserName)
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

    public async Task<MemberDto> GetMemberByNameAsync(string name)
    {
        // do not need to .Include(u=> u.Photo), mapper and EF will perform left join
        return await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(u => u.Name.Equals(name));
    }
}