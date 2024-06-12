using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TestCreator.Data.Models;
using TestCreator.WebApp.Helpers;
using TestCreator.WebApp.Services.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TestCreator.WebApp.Services
{
    public class TokenService : ITokenService
    {
        private IConfiguration Configuration { get; }

        public TokenService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Claim[] CreateClaims(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentException("UserId can't be NULL");
            }

            DateTime now = DateTime.UtcNow;

            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeMilliseconds().ToString())
            };
        }

        public Token GenerateRefreshToken(string clientId, string userId)
        {
            if (userId == null || clientId == null) 
            {
                throw new ArgumentException("Arguments can't be NULL");
            }

            return new Token
            {
                ClientId = clientId,
                UserId = userId,
                Type = 0,
                Value = Guid.NewGuid().ToString("N"),
                CreationDate = DateTime.Now
            };
        }

        public TokenData CreateAccessToken(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentException("UserId can't be NULL");
            }

            var claims = CreateClaims(userId);

            return CreateSecurityToken(claims);
        }

        public TokenData CreateSecurityToken(Claim[] claims)
        {
            if (claims == null)
            {
                throw new ArgumentException("Claims can't be NULL");
            }

            DateTime now = DateTime.UtcNow;
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));
            var tokenExpirationalMins = Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");

            var token = new JwtSecurityToken(
                issuer: Configuration["Auth:Jwt:Issuer"],
                audience: Configuration["Auth:Jwt:Audience"],
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(tokenExpirationalMins)),
                signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256));

            return new TokenData
            {
                EncodedToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExporationTimeInMinutes = tokenExpirationalMins
            };
        }
    }
}
