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

    public UsersController(IUserService userRepo)
    {
        _userRepo = userRepo;
    }


    // GET: api/<UsersController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsersAsync()
    {
        var users = await _userRepo.GetAllUsersAsync();
        return Ok(users);
    }

    // GET api/<UsersController>/5
    [HttpGet("search-id/{id}")]
    public async Task<ActionResult<AppUser>> GetUserAsync(Guid id)
    {
        var user = await _userRepo.GetUserByIdAsync(id);
        return user is not null ? Ok(user) : NotFound();
    }

    // GET api/<UsersController>/5
    [HttpGet("{name}")]
    public async Task<ActionResult<AppUser>> GetUserByNameAsync(string name)
    {
        var user = await _userRepo.GetUserByNameAsync(name);
        return user is not null ? Ok(user) : NotFound();
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