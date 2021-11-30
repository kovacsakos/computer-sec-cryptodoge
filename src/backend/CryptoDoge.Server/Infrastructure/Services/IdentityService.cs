using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CryptoDoge.Server.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpContext context;
        private readonly UserManager<User> userManager;

        public IdentityService(IHttpContextAccessor httpContextAccessor,
                                UserManager<User> userManager)
        {
            this.context = httpContextAccessor.HttpContext;
            this.userManager = userManager;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var id = context.User.Claims.Single(x => x.Type == JwtClaimTypes.Id).Value;
            return await userManager.FindByIdAsync(id);
        }

        public Task<string> GetCurrentUserIdAsync()
        {
            return Task.FromResult(context.User.Claims.SingleOrDefault(x => x.Type == "id")?.Value);
        }
    }
}
