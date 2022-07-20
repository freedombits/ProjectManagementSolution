using ProjectManagementSolution.API.Features.Users;
using System.Security.Claims;

namespace ProjectManagementSolution.API.Authentication;

public interface IUserClaimsPrincipalFactory
{
    ClaimsPrincipal CreateClaimsPrincipal(User user);
}
