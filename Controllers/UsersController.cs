using Microsoft.AspNetCore.Mvc;
using E_CH_back.Models;
using E_CH_back.Services;

namespace E_CH_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (await _userService.UserExists(user.Email))
            {
                return Conflict(new { message = "User already exists" });
            }

            var userId = await _userService.AddUser(user);
            return CreatedAtAction(nameof(AddUser), new { id = userId }, new { userId });
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string email)
        {
            var exists = await _userService.UserExists(email);
            return Ok(new { exists });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            var isAuthenticated = await _userService.AuthenticateUser(user.Email, user.Password);
            if (!isAuthenticated)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new { message = "Authenticated successfully" });
        }
    }
}
