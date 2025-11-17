using StajPortal.Models.Entities;

namespace StajPortal.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}

