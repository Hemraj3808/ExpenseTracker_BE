using Auth.Domain.Entities;
using Auth.Application.Dtos;


namespace Auth.Application.Services
{
    public interface IJwtTokenGenerator
    {
        AuthResponseDto GenerateJwtToken(ApplicationUser users);
    }
}
