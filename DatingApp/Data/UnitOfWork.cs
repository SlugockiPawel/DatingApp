using AutoMapper;
using DatingApp.Services;
using DatingApp.Services.Interfaces;

namespace DatingApp.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public UnitOfWork(ApplicationDbContext context, IMapper mapper, ITokenService tokenService)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public IUserService UserService => new UserServicePostgres(_context, _tokenService, _mapper);
    public IMessageService MessageService => new MessageService(_context, _mapper);
    public ILikeService LikeService => new LikeService(_context);

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}