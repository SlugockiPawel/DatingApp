using DatingApp.Data;
using DatingApp.Models;
using DatingApp.Repos.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAppUserRepo _userRepo;

        public UsersController(IAppUserRepo userRepo)
        {
            _userRepo = userRepo;
        }


        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsersAsync()
        {
            var users = await _userRepo.GetAllUsersAsync();
            return Ok(users);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserAsync(Guid id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
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
}
