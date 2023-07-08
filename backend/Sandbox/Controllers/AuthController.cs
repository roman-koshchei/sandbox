using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sandbox.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<SandboxUser> userManager;

        public AuthController(UserManager<SandboxUser> userManager)
        {
            this.userManager = userManager;
        }

        public class RegisterInput
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterInput input)
        {
            input.Password = input.Password.Trim();
            if (string.IsNullOrEmpty(input.Password)) return BadRequest("Password can't be empty");

            input.ConfirmPassword = input.ConfirmPassword.Trim();
            if (input.Password != input.ConfirmPassword) return BadRequest("Confirm password isn't correct");

            input.Email = input.Email.Trim();
            if (!string.IsNullOrEmpty(input.Email)) return BadRequest("Email isn't valid");

            SandboxUser user = new(input.Email, input.Name.Trim());

            var operation = await userManager.CreateAsync(user, input.Password);

            if (!operation.Succeeded) return Problem(); // check errors

            // set cookie

            return Ok();
        }

        public class LoginInput
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Login(HttpResponse response, [FromBody] LoginInput input)
        {
            input.Email = input.Email.Trim();
            if (string.IsNullOrEmpty(input.Email)) return BadRequest("Email can't be empty.");

            input.Password = input.Password.Trim();
            if (string.IsNullOrEmpty(input.Password)) return BadRequest("Password can't be empty.");

            var user = await userManager.FindByEmailAsync(input.Email);
            if (user == null) return BadRequest($"User with email: {input.Email} doesn't exist.");

            var isValid = await userManager.CheckPasswordAsync(user, input.Password);
            if (!isValid) return BadRequest("Password is incorrect.");

            // set cookie
            response.Cookies.Append("location", "token", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
            });

            return Ok();
        }
    }
}