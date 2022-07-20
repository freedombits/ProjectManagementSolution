using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSolution.API.DataAccess;
using ProjectManagementSolution.API.Features.Emails;

namespace ProjectManagementSolution.API.Features.Users;

[ApiController]
[Route("api/users")]
public class PasswordResetController : ControllerBase
{
    private readonly IMediator _mediator;

    public PasswordResetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("reset_password")]
    public async Task<IActionResult> SendPasswordResetAsync([FromBody] PasswordResetRequest forgotPasswordRequest)
    {
        _ = await _mediator.Send(forgotPasswordRequest);
        return NoContent();
    }
}

public record PasswordResetRequest(string EmailAddress) : IRequest<Unit>;

public class PasswordResetRequestValidator : AbstractValidator<PasswordResetRequest>
{
    public PasswordResetRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotNull().WithMessage("Email address must not be null.")
            .NotEmpty().WithMessage("Email Address must not be empty.")
            .EmailAddress().WithMessage("Email address must be a valid.");
    }
}

public class PasswordResetRequestHandler : IRequestHandler<PasswordResetRequest>
{
    private readonly IMediator _mediator;
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<PasswordResetRequestHandler> _logger;

    public PasswordResetRequestHandler(
        IMediator mediator,
        DatabaseContext databaseContext,
        ILogger<PasswordResetRequestHandler> logger)
    {
        _mediator = mediator;
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(PasswordResetRequest request, CancellationToken cancellationToken)
    {
        var user = await _databaseContext.Users
            .FirstOrDefaultAsync(user => user.EmailAddress.ToLower() == request.EmailAddress.ToLower(), cancellationToken);

        if (user is null)
        {
            return Unit.Value;
        }

        var passwordResetToken = user.CreatePasswordResetToken();

        _databaseContext.PasswordResetTokens.Add(passwordResetToken);
        await _databaseContext.SaveChangesAsync(cancellationToken);

        var passwordResetEmail = new PasswordResetEmail(passwordResetToken.Id);
        await _mediator.Publish(new EmailNotification(passwordResetEmail), cancellationToken);

        return Unit.Value;
    }
}
