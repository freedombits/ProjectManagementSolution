using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSolution.API.Authentication;
using ProjectManagementSolution.API.DataAccess;
using ProjectManagementSolution.API.Features.Emails;
using System.IdentityModel.Tokens.Jwt;

namespace ProjectManagementSolution.API.Features.Users;

[ApiController]
[Route("api/users")]
public class ResendVerificationEmail : ControllerBase
{
    private readonly IMediator _mediator;

    public ResendVerificationEmail(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("resend_verification_email")]
    public async Task<IActionResult> ResendVerificationEmailAsync()
    {
        _ = await _mediator.Send(new ResendVerificationEmailRequest());
        return NoContent();
    }
}

public record ResendVerificationEmailRequest() : IRequest<Unit>;

public class ResendVerificationEmailRequestHandler : IRequestHandler<ResendVerificationEmailRequest, Unit>
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<ResendVerificationEmailRequestHandler> _logger;

    public ResendVerificationEmailRequestHandler(IMediator mediator,
        IUserService userService,
        DatabaseContext databaseContext,
        ILogger<ResendVerificationEmailRequestHandler> logger)
    {
        _mediator = mediator;
        _userService = userService;
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(ResendVerificationEmailRequest request, CancellationToken cancellationToken)
    {
        var currentUser = await _userService.GetCurrentUserAsync();

        if (currentUser is null)
        {
            return Unit.Value;
        }

        // TODO: repeated code.
        var emailVerificationToken = currentUser.CreateEmailVerificationToken();

        currentUser.EmailVerificationTokens.Add(emailVerificationToken);
        await _databaseContext.SaveChangesAsync(cancellationToken);

        try
        {
            var accountVerificationEmail = new AccountVerificationEmail(emailVerificationToken.Token);
            await _mediator.Publish(new EmailNotification(accountVerificationEmail), cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Failed to send account verification email to {UserId}", currentUser.Id);
        }

        throw new NotImplementedException();
    }
}
