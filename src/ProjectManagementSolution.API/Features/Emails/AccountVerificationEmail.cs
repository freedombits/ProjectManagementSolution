using Microsoft.AspNetCore.Server.IIS.Core;

namespace ProjectManagementSolution.API.Features.Emails;

public class AccountVerificationEmail : IEmail
{
    public string Subject { get; } = "Verify your email address.";

    public string Body { get; }

    public AccountVerificationEmail(Guid confirmationToken)
    {
        // TODO: Implement this.
        Body = string.Empty;
    }
}
