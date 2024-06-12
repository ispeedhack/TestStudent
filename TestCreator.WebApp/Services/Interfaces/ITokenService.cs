using System.Security.Claims;
using TestCreator.Data.Models;
using TestCreator.WebApp.Helpers;

namespace TestCreator.WebApp.Services.Interfaces
{
    public interface ITokenService
    {
        Claim[] CreateClaims(string userId);
        TokenData CreateSecurityToken(Claim[] claims);
        Token GenerateRefreshToken(string clientId, string userId);
        TokenData CreateAccessToken(string userId);
    }
}
