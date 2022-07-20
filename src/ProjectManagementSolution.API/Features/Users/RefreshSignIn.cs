using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSolution.API.Authentication;

namespace ProjectManagementSolution.API.Features.Users;

[ApiController]
public class RefreshSignIn : ControllerBase
{
    private readonly IMediator _mediator;

    public RefreshSignIn(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> RefreshSignInAsync()
    {
        _ = await _mediator.Send(new RefreshSignInRequest());
        return NoContent();
    }
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