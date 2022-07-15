using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;


        public UserServicePostgres(ApplicationDbContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }


        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Photos)
                .ToListAsync();
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

            return new UserDto()
            {
                Name = user.Name,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
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

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<MemberDto> GetMemberByNameAsync(string name)
        {
            // do not need to .Include(u=> u.Photo), mapper and EF will perform left join
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(u => u.Name.Equals(name));
        }
    }
}