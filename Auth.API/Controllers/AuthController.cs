using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Auth.Application.Dtos;
using Auth.Application.Services;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) 
            
                return Ok("User registered successfully");

            return BadRequest(result.Errors);

            
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request , [FromServices] IJwtTokenGenerator tokenGenerator)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Invalid Email or Password");
            
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
                return Unauthorized("Invalid Email or Password");

            var token = tokenGenerator.GenerateJwtToken(user);
            return Ok(token);
        }
    }
}
