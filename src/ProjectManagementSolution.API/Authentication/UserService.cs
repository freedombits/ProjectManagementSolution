using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSolution.API.DataAccess;
using ProjectManagementSolution.API.Features.Users;
using System.Security.Claims;

namespace ProjectManagementSolution.API.Authentication;

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUserClaimsPrincipalFactory _userClaimsPrincipalFactory;

    public UserService(
        DatabaseContext databaseContext,
        IHttpContextAccessor httpContextAccessor,
        IUserClaimsPrincipalFactory userClaimsPrincipalFactory)
    {
        _databaseContext = databaseContext;
        _contextAccessor = httpContextAccessor;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var httpContext = _contextAccessor.HttpContext!;
        var userIdValue = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdValue))
        {
            return null;
        }

        var userId = Guid.Parse(userIdValue);
        return await _databaseContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
    }

    public async Task RefreshSignInAsync(User user)
    {
        var httpContext = _contextAccessor.HttpContext!;
        var authenticationResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (authenticationResult is not { Succeeded: true })
        {
            return;
        }

        await SignInAsync(user, authenticationResult.Properties);
    }

    public async Task SignInAsync(User user, bool isPersistent)
    {
        await SignInAsync(
            user,
            new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                IssuedUtc = DateTimeOffset.UtcNow
            });
    }

    public async Task SignInAsync(User user, AuthenticationProperties authenticationProperties)
    {
        var httpContext = _contextAccessor.HttpContext!;
        var claimsPrincipal = _userClaimsPrincipalFactory.CreateClaimsPrincipal(user);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authenticationProperties);
    }
}
