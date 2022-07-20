using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSolution.API.DataAccess;
using ProjectManagementSolution.API.Features.Emails;

namespace ProjectManagementSolution.API.Features.Users;

[ApiController]
[ApiVersion("1.0")]
[Route("api/users")]
public class CreateAccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateAccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create_account")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateAccountRequest registerRequest)
    {
        return Ok(await _mediator.Send(registerRequest));
    }
}

public record CreateAccountRequest(string Username, string EmailAddress, string Password) : IRequest<CreateAccountResponse>;
public record CreateAccountResponse(bool Success, IEnumerable<string> Errors);

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    private bool ContainsUppercase(string s) => s.Any(char.IsUpper);
    private bool ContainsNumber(string s) => s.Any(char.IsNumber);
    private bool ContainsSymbol(string s) => s.Any(c => "!@#$%^&*()_-+=|\\}]{[\"':;<,>.?/".Contains(c));

    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotNull().WithMessage("Username must not be null.")
            .NotEmpty().WithMessage("Username must not be empty.");

        RuleFor(x => x.EmailAddress)
            .NotNull().WithMessage("EmailAddress must not be null.")
            .NotEmpty().WithMessage("EmailAddress must not be empty.")
            .EmailAddress().WithMessage("EmailAddress must be an email address.");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Password must not be null.")
            .NotEmpty().WithMessage("Password must not be empty.")
            .Must(ContainsUppercase).WithMessage("Password must contain an uppercase character.")
            .Must(ContainsNumber).WithMessage("Password must contain a number.")
            .Must(ContainsSymbol).WithMessage("Password must contain a symbol.");
    }
}

public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, CreateAccountResponse>
{
    private readonly IMediator _mediator;
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<CreateAccountRequestHandler> _logger;

    public CreateAccountRequestHandler(IMediator mediator,
        DatabaseContext databaseContext,
        ILogger<CreateAccountRequestHandler> logger)
    {
        _mediator = mediator;
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        IList<string> errors = Array.Empty<string>();

        // TODO: is this any good?
        if (await _databaseContext.Users
            .AnyAsync(user => user.Username.ToLower() == request.Username.ToLower(), cancellationToken))
        {
            errors.Add("A user with that username already eixsts.");
        }

        if (await _databaseContext.Users
            .AnyAsync(user => user.EmailAddress.ToLower() == request.EmailAddress.ToLower(), cancellationToken))
        {
            errors.Add("A user with that email address already exists.");
        }

        if (errors.Any())
        {
            return new CreateAccountResponse(false, errors);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(request.Username, passwordHash, request.EmailAddress);
        var verificationToken = user.CreateEmailVerificationToken();
        user.EmailVerificationTokens.Add(verificationToken);

        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync(cancellationToken);

        var accountVerificationEmail = new AccountVerificationEmail(verificationToken.Token);
        await _mediator.Publish(new EmailNotification(accountVerificationEmail), cancellationToken);

        return new CreateAccountResponse(true, errors);
    }
}
