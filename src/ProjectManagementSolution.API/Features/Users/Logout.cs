using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSolution.API.Authentication;

namespace ProjectManagementSolution.API.Features.Users;

[ApiController]
[Route("api/users")]
public class Logout : ControllerBase
{
    private readonly IMediator _mediator;

    public Logout(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        _ = await _mediator.Send(new LogoutRequest());
        return NoContent();
    }
}

public record LogoutRequest() : IRequest<Unit>;

public class LogoutRequestHandler : IRequestHandler<LogoutRequest, Unit>
{
    private readonly IUserService _userService;

    public LogoutRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Unit> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        await _userService.LogoutAsync();
        return Unit.Value;
    }
}
