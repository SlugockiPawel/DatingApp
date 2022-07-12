﻿using System.Security.Claims;
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
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        // var users = await _userRepo.GetAllUsersAsync();
        var mappedUsers = await _userRepo.GetMembersAsync();

        return Ok(mappedUsers);
    }

    // GET api/<UsersController>/5
    [HttpGet("search-id/{id:guid}")]
    public async Task<ActionResult<MemberDto>> GetUser(Guid id)
    {
        // old style, to show how to map using autoMapper
        var user = await _userRepo.GetUserByIdAsync(id);
        var mappedUser = _mapper.Map<MemberDto>(user);

        return mappedUser is not null ? Ok(mappedUser) : NotFound();
    }

    // GET api/<UsersController>/5
    [HttpGet("{name}", Name = "GetUserByName")]
    public async Task<ActionResult<MemberDto>> GetUserByName(string name)
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
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
        };

        if (user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }

        user.Photos.Add(photo);

        if (await _userRepo.SaveAllAsync())
        {
            return CreatedAtRoute(nameof(GetUserByName), new { name = user.Name }, _mapper.Map<PhotoDto>(photo));
            // return _mapper.Map<PhotoDto>(photo);
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _userRepo.GetUserByNameAsync(User.GetUserName());
        var photo = user.Photos.FirstOrDefault(p => p.Id.Equals(photoId));

        if (photo is null)
        {
            return NotFound();
        }

        if (photo.IsMain) return BadRequest("This Photo is already a main photo");

        var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
        if (currentMain is not null) currentMain.IsMain = false;

        photo.IsMain = true;

        if (await _userRepo.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to set main photo");
    }
}