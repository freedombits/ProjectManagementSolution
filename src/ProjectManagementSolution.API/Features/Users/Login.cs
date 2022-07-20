using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSolution.API.Authentication;
using ProjectManagementSolution.API.DataAccess;
using static BCrypt.Net.BCrypt;

namespace ProjectManagementSolution.API.Features.Users;

[ApiController]
[ApiVersion("1.0")]
[Route("api/users")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest loginRequest)
    {
        return Ok(await _mediator.Send(loginRequest));
    }
}

public record UserLoginRequest(string Username, string Password, bool RememberMe) : IRequest<UserLoginResponse>;
public record UserLoginResponse(bool Success);

public class UserLoginRequestHandler : IRequestHandler<UserLoginRequest, UserLoginResponse>
{
    private readonly IUserService _userService;
    private readonly DatabaseContext _databaseContext;

    public UserLoginRequestHandler(
        IUserService userService,
        DatabaseContext databaseContext)
    {
        _userService = userService;
        _databaseContext = databaseContext;
    }

    public async Task<UserLoginResponse> Handle(UserLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == request.Username.ToLower(), cancellationToken);

        if (user is null || !Verify(request.Password, user.PasswordHash))
        {
            return new UserLoginResponse(false);
        }

        await _userService.SignInAsync(user, request.RememberMe);
        return new UserLoginResponse(true);
    }
}
