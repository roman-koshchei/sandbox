using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sandbox.Auth;
using Sandbox.Lib;
using System.ComponentModel.DataAnnotations;
using Unator.Email;

namespace Sandbox.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<SandboxUser> userManager;
        private readonly Jwt jwt;
        private readonly UEmailSender emailGod;

        public AuthController(UserManager<SandboxUser> userManager, Jwt jwt, UEmailSender emailGod)
        {
            this.userManager = userManager;
            this.jwt = jwt;
            this.emailGod = emailGod;
        }

        [Authorize]
        [HttpPost("me")]
        public IActionResult GetSomething()
        {
            var uid = User.FindFirst(Jwt.Uid)?.Value;
            return Ok(uid);
        }

        public class RegisterInput
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;

            [Required]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required]
            public string Name { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput input)
        {
            input.Password = input.Password.Trim();
            if (string.IsNullOrEmpty(input.Password)) return BadRequest("Password can't be empty");

            input.ConfirmPassword = input.ConfirmPassword.Trim();
            if (input.Password != input.ConfirmPassword) return BadRequest("Confirm password isn't correct");

            input.Email = input.Email.Trim();
            if (string.IsNullOrEmpty(input.Email)) return BadRequest("Email isn't valid");

            SandboxUser user = new(input.Email, input.Name.Trim());

            var operation = await userManager.CreateAsync(user, input.Password);

            if (!operation.Succeeded) return Problem(); // check errors

            var verificationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var verificationEmail = await emailGod.Send("roman@paragoda.tech", "Sandbox", new List<string>() { user.Email },
                subject: "Verify email at Sandbox by Roman Koshchei",
                text: $"Use link to verify: https://localhost/api/auth/verify?token={verificationToken}",
                html: $"<a href='https://localhost/api/auth/verify?token={verificationToken}'>Click to verify</a>"
            );

            if (verificationEmail != EmailStatus.Success)
            {
                // message about need to manually verify?
                // or rollback user creation?
            }

            var token = jwt.Token(user.Id, user.Version);

            Response.Cookies.Append(RefreshOnly.Cookie, token, new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddDays(30)
            });

            return Ok();
        }

        public class LoginInput
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input)
        {
            input.Email = input.Email.Trim();
            if (string.IsNullOrEmpty(input.Email)) return BadRequest("Email can't be empty.");

            input.Password = input.Password.Trim();
            if (string.IsNullOrEmpty(input.Password)) return BadRequest("Password can't be empty.");

            var user = await userManager.FindByEmailAsync(input.Email);
            if (user == null) return BadRequest($"User with email: {input.Email} doesn't exist.");

            var isValid = await userManager.CheckPasswordAsync(user, input.Password);
            if (!isValid) return BadRequest("Password is incorrect.");

            var token = jwt.Token(user.Id, user.Version);

            Response.Cookies.Append(RefreshOnly.Cookie, token, new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddDays(30)
            });
            return Ok();
        }
    }
}