using System.ComponentModel.DataAnnotations;

namespace DatingApp.DTOs;

public sealed class LoginDto
{
    [Required] public string UserName { get; set; }

    [Required] public string Password { get; set; }
}