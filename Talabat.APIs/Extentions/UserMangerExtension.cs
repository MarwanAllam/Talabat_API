using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.Apis.Extentions
{
    public static class UserMangerExtension
    {
        public static async Task<AppUser?> FindUserWithAddressAsync(this UserManager<AppUser>userManger,ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManger.Users.Include(U=>U.Address).SingleOrDefaultAsync(U=>U.Email==email);
            return user;
        }
    }
}
