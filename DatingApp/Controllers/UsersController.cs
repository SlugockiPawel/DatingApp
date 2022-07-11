using System.Security.Claims;
using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingApp.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userRepo;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserService userRepo, IMapper mapper, IPhotoService photoService)
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _photoService = photoService;
    }

    // GET: api/<UsersController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsersAsync()
    {
        // var users = await _userRepo.GetAllUsersAsync();
        var mappedUsers = await _userRepo.GetMembersAsync();

        return Ok(mappedUsers);
    }

    // GET api/<UsersController>/5
    [HttpGet("search-id/{id}")]
    public async Task<ActionResult<MemberDto>> GetUserAsync(Guid id)
    {
        // old style, to show how to map using autoMapper
        var user = await _userRepo.GetUserByIdAsync(id);
        var mappedUser = _mapper.Map<MemberDto>(user);

        return mappedUser is not null ? Ok(mappedUser) : NotFound();
    }

    // GET api/<UsersController>/5
    [HttpGet("{name}")]
    public async Task<ActionResult<MemberDto>> GetUserByNameAsync(string name)
    {
        // we map entity using autoMapper directly in the repo query

        // var user = await _userRepo.GetUserByNameAsync(name);
        var mappedUser = await _userRepo.GetMemberByNameAsync(name);

        return mappedUser is not null ? Ok(mappedUser) : NotFound();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        // get username based on the token value
        var user = await _userRepo.GetUserByNameAsync(User.GetUserName());

        _mapper.Map(memberUpdateDto, user);
        _userRepo.Update(user);

        return await _userRepo.SaveAllAsync() ? NoContent() : BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userRepo.GetUserByNameAsync(User.GetUserName());
        var result = await _photoService.AddPhotoAsync(file);
        if (result.Error is not null) return BadRequest(result.Error.Message);

        var photo = new Photo()
        {
            Url = result.SecureUrl.AbsolutePath,
            PublicId = result.PublicId,
        };

        if (user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }
        
        user.Photos.Add(photo);

        if (await _userRepo.SaveAllAsync())
        {
            return _mapper.Map<PhotoDto>(photo);
        }

        return BadRequest("Problem adding photo");
    }
    
}