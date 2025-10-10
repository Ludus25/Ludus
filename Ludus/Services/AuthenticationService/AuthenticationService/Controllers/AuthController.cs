using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AuthenticationService.Entities;
using AuthenticationService.Validators;
using AuthenticationService.Services;



namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!MlbValidator.IsValidJMBG(model.Mlb))
            {
                return BadRequest("Invalid Mlb (JMBG) format.");
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                mlb = model.Mlb,
                name = model.Name,
                surname = model.Surname,
                TwoFactorEnabled = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);



            if (model.Enable2FA)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                await _emailSender.SendEmailAsync(user.Email, "2FA Code", $"Your 2FA verification code is: {token}");
            }

            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { Message = "User successfully registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);


            if (model.Email.Contains("adm", StringComparison.OrdinalIgnoreCase))
            {
                bool valid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                if (!valid)
                    return Unauthorized("Invalid credentials");
            }
            else
            {

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                    return Unauthorized("Invalid credentials");
            }

            if (user.TwoFactorEnabled)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                await _emailSender.SendEmailAsync(user.Email, "2FA Code", $"Your verification code is: {token}");

                return Ok(new
                {
                    Requires2FA = true,
                    Email = user.Email,
                    Message = "2FA code sent via email"
                });
            }


            var jwt = GenerateJwtToken(user);
            return Ok(new { Token = jwt });
        }


        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FAModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, TokenOptions.DefaultEmailProvider, model.Code);

            if (!isValid)
                return BadRequest("Invalid or expired 2FA code");

            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

            var jwt = GenerateJwtToken(user);
            return Ok(new { Token = jwt });
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                //new Claim(JwtRegisteredClaimNames.Role, user.),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var userRoles = _userManager.GetRolesAsync(user).Result;

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest("User not found with the provided email.");
            }


            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("New password and confirm password do not match");
            }


            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password changed successfully!");
        }

    }

}

