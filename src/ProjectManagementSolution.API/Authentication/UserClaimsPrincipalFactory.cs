using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectManagementSolution.API.Features.Users;
using System.Security.Claims;

namespace ProjectManagementSolution.API.Authentication;

public class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory
{
    public ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.EmailAddress),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(claimsIdentity);
    }
}
