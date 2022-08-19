using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingApp.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IMapper _mapper;
    
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    // GET: api/<UsersController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(
        [FromQuery] UserParams userParams
    )
    {
        var username = User.GetUserName();
        var gender = await _unitOfWork.UserService.GetUserGender(username);
        userParams.CurrentUserName = username;

        if (string.IsNullOrWhiteSpace(userParams.Gender))
            userParams.Gender = gender == "male" ? "female" : "male";

        var users = await _unitOfWork.UserService.GetMembersAsync(userParams);

        Response.AddPaginationHeader(
            users.CurrentPage,
            users.PageSize,
            users.TotalCount,
            users.TotalPages
        );

        return Ok(users);
    }

    // GET api/<UsersController>/5
    [HttpGet("search-id/{id:guid}")]
    public async Task<ActionResult<MemberDto>> GetUser(Guid id)
    {
        // old style, to show how to map using autoMapper
        var user = await _unitOfWork.UserService.GetUserByIdAsync(id);
        var mappedUser = _mapper.Map<MemberDto>(user);

        return mappedUser is not null ? Ok(mappedUser) : NotFound();
    }

    // GET api/<UsersController>/5
    [HttpGet("{name}", Name = "GetUserByName")]
    public async Task<ActionResult<MemberDto>> GetUserByName(string name)
    {
        var isCurrentUser = User.GetUserName() == name;
        var mappedUser = await _unitOfWork.UserService.GetMemberByNameAsync(name, isCurrentUser);

        return mappedUser is not null ? Ok(mappedUser) : NotFound();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        // get username based on the token value
        var user = await _unitOfWork.UserService.GetUserByNameAsync(User.GetUserName());

        _mapper.Map(memberUpdateDto, user);
        _unitOfWork.UserService.Update(user);

        return await _unitOfWork.Complete() ? NoContent() : BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _unitOfWork.UserService.GetUserByNameAsync(User.GetUserName());
        var result = await _unitOfWork.PhotoService.AddPhotoAsync(file);
        if (result.Error is not null)
            return BadRequest(result.Error.Message);

        var photo = new Photo { Url = result.SecureUrl.AbsoluteUri, PublicId = result.PublicId };

        if (user.Photos.Count == 0)
            photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _unitOfWork.Complete())
            return CreatedAtRoute(
                nameof(GetUserByName),
                new { name = user.UserName },
                _mapper.Map<PhotoDto>(photo)
            );
        // return _mapper.Map<PhotoDto>(photo);

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _unitOfWork.UserService.GetUserByNameAsync(User.GetUserName());
        var photo = user.Photos.FirstOrDefault(p => p.Id.Equals(photoId));

        if (photo is null)
            return NotFound();

        if (photo.IsMain)
            return BadRequest("This Photo is already a main photo");

        var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
        if (currentMain is not null)
            currentMain.IsMain = false;

        photo.IsMain = true;

        if (await _unitOfWork.Complete())
            return NoContent();

        return BadRequest("Failed to set main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _unitOfWork.UserService.GetUserByNameAsync(User.GetUserName());
        var photo = user.Photos.FirstOrDefault(p => p.Id.Equals(photoId));

        if (photo is null)
            return NotFound();

        if (photo.IsMain)
            return BadRequest("You cannot delete main photo");

        if (photo.PublicId is not null)
        {
            var result = await _unitOfWork.PhotoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error is not null)
                return BadRequest(result.Error.Message);

            user.Photos.Remove(photo);

            if (await _unitOfWork.Complete())
                return Ok();
        }

        return BadRequest("Failed to delete the photo");
    }
}