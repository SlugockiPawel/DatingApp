using System.Security.Claims;
using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
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

    public UsersController(IUserService userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
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
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userRepo.GetUserByNameAsync(username);

        _mapper.Map(memberUpdateDto, user);
        _userRepo.Update(user);

        return await _userRepo.SaveAllAsync() ? NoContent() : BadRequest("Failed to update user");
    }


    // POST api/<UsersController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // DELETE api/<UsersController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}