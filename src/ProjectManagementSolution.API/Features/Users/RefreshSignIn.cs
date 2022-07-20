using MediatR;
using ProjectManagementSolution.API.Authentication;

namespace ProjectManagementSolution.API.Features.Users;

public class RefreshSignIn
{
}

public record RefreshSignInRequest() : IRequest<Unit>;

public class RefreshSignInRequestHandler : IRequestHandler<RefreshSignInRequest, Unit>
{
    private readonly IUserService _userService;

    public RefreshSignInRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Unit> Handle(RefreshSignInRequest request, CancellationToken cancellationToken)
    {
        var currentUser = await _userService.GetCurrentUserAsync();

        if (currentUser is null)
        {
            return Unit.Value;
        }

        await _userService.RefreshSignInAsync(currentUser);
        return Unit.Value;
    }
}