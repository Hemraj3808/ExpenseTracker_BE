using Auth.Application.Dtos;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Infrastructure.Services
{
    public class JwtTockenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        

        public  JwtTockenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        AuthResponseDto IJwtTokenGenerator.GenerateJwtToken(ApplicationUser users)
        {
            return GenerateToken(users);
        }

        public AuthResponseDto GenerateToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var duration = Convert.ToInt32(jwtSettings["durationInMinutes"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id),
                new Claim(ClaimTypes.Name ,user.UserName ??""),
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds =  new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: creds
                );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }
    }
}
