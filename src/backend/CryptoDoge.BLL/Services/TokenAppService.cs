using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.Shared;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Services
{
    public class TokenAppService : ITokenAppService
    {
        private readonly IConfiguration configuration;
        private readonly IAuthRepository userRepository;
        private readonly UserManager<User> userManager;
        private readonly ICryptoRandomGenerator randomGenerator;

        public TokenAppService(IConfiguration configuration,
                               IAuthRepository userRepository,
                               UserManager<User> userManager,
                               ICryptoRandomGenerator randomGenerator)
        {
            this.configuration = configuration;
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.randomGenerator = randomGenerator;
        }

        public Task<string> CreateAccessToken(User user)
        {
            var userRoles = userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Id, user.Id),
            };

            claims.AddRange(userRoles.Select(role => new Claim(JwtClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Authentication")["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = configuration.GetSection("Authentication")["JwtDefaultIssuer"];

            if (userManager.IsInRoleAsync(user, "ADMIN").Result)
            {
                issuer = configuration.GetSection("Authentication")["JwtAdminIssuer"];
            }

            var token = new JwtSecurityToken(
               issuer,
               issuer,
               claims,
               expires: DateTime.Now.AddMinutes(Convert.ToInt32(configuration.GetSection("Authentication")["JwtExpirationMinutes"])),
               signingCredentials: creds
            );
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<string> CreateRefreshTokenAsync(User user)
        {
            string refreshToken = randomGenerator.GenerateRandomString();
            await userRepository.SaveRefreshTokenAsync(user, refreshToken);
            return refreshToken;
        }

        public async Task RemoveRefreshToken(string userId)
        {
            await userRepository.RemoveRefreshTokenAsync(userId);
        }
    }
}
