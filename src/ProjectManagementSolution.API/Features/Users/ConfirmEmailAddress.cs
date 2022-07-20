using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSolution.API.DataAccess;
using ProjectManagementSolution.API.Responses;

namespace ProjectManagementSolution.API.Features.Users;

[ApiController]
[Route("api/users")]
public class ConfirmEmailAddress : ControllerBase
{
    private readonly IMediator _mediator;

    public ConfirmEmailAddress(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("confirm_email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest confirmEmailRequest)
    {
        var response = await _mediator.Send(confirmEmailRequest);
        return response.ToActionResult();
    }
}

public record ConfirmEmailRequest(Guid TokenId) : IRequest<IToActionResult>;
public record ConfirmEmailResponse(bool Success, IEnumerable<string> Errors);

public class ConfirmEmailRequestHandler : IRequestHandler<ConfirmEmailRequest, IToActionResult>
{
    private readonly DatabaseContext _databaseContext;

    public ConfirmEmailRequestHandler(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IToActionResult> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var verificationToken = await _databaseContext.EmailVerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.TokenId, cancellationToken);

        if (verificationToken is null)
        {
            return new StatusCodeResponse(StatusCodes.Status404NotFound);
        }

        if (!verificationToken.Active)
        {
            return new StatusCodeResponse<ConfirmEmailResponse>(StatusCodes.Status200OK,
                new ConfirmEmailResponse(false, new[] { "Token has already been used." }));
        }

        if (verificationToken.IsExpired())
        {
            return new StatusCodeResponse<ConfirmEmailResponse>(StatusCodes.Status200OK,
                new ConfirmEmailResponse(false, new[] { "Token is expired." }));
        }

        verificationToken.Deactivate();
        verificationToken.User.Verify();

        await _databaseContext.SaveChangesAsync(cancellationToken);

        return new StatusCodeResponse<ConfirmEmailResponse>(StatusCodes.Status200OK,
            new ConfirmEmailResponse(true, Enumerable.Empty<string>()));
    }
}