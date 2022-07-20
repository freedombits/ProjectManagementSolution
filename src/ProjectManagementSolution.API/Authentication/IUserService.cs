using Microsoft.AspNetCore.Authentication;
using ProjectManagementSolution.API.Features.Users;

namespace ProjectManagementSolution.API.Authentication;

public interface IUserService
{
    Task<User?> GetCurrentUserAsync();
    Task RefreshSignInAsync(User user);
    Task SignInAsync(User user, AuthenticationProperties authenticationProperties);
    Task SignInAsync(User user, bool isPersistent);
    Task LogoutAsync();
}
