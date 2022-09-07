using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services;

public class LikeService : ILikeService
{
    private readonly ApplicationDbContext _context;

    public LikeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppUserLike> GetUserLike(Guid sourceUserId, Guid likedUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<AppUser> GetUserWithLikes(Guid userId)
    {
        return await _context.Users
            .Include(u => u.LikedUsers)
            .FirstOrDefaultAsync(u => u.Id.Equals(userId));
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();
        IQueryable<LikeDto> likedUsers;
        var loggedUserLikes = likes.Where(like => like.SourceUserId.Equals(likesParams.UserId));

        switch (likesParams.Predicate)
        {
            case "liked":
                likes = loggedUserLikes;
                users = likes.Select(like => like.LikedUser);
                break;
            case "likedBy":
                likes = likes.Where(like => like.LikedUserId.Equals(likesParams.UserId));
                users = likes.Select(like => like.SourceUser);
                break;
        }

        likedUsers = AddLikedByCurrentUserFlag(
            likesParams.Predicate,
            users,
            loggedUserLikes,
            likesParams.UserId
        );

        return await PagedList<LikeDto>.CreateAsync(
            likedUsers,
            likesParams.PageNumber,
            likesParams.PageSize
        );
    }

    private static IQueryable<LikeDto> AddLikedByCurrentUserFlag(
        string predicate,
        IQueryable<AppUser> users,
        IQueryable<AppUserLike> loggedUserLikes,
        Guid loggedUserId
    )
    {
        if (predicate == "liked")
        {
            return users.Select(
                u =>
                    new LikeDto
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        KnownAs = u.KnownAs,
                        Age = u.DateOfBirth.CalculateAge(),
                        PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url,
                        City = u.City,
                        LikedByCurrentUser = true
                    }
            );
        }

        return users.Select(
            u =>
                new LikeDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    KnownAs = u.KnownAs,
                    Age = u.DateOfBirth.CalculateAge(),
                    PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url,
                    City = u.City,
                    LikedByCurrentUser = u.LikedByUsers.Any(ul => ul.SourceUserId == loggedUserId)
                }
        );
    }
}