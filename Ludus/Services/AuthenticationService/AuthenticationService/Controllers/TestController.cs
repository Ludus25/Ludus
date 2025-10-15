using AuthenticationService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly IUserRepository _userRepository;

        public TestController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("names")]
        public async Task<IActionResult> GetNames()
        {

            var names = await _userRepository.GetAllUsers();
            return Ok(names);
        }
    }
}
