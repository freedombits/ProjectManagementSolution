namespace ProjectManagementSolution.API.Features.Emails;

public class PasswordResetEmail : IEmail
{
    public string Subject { get; } = "Reset your password.";

    public string Body { get; }

    public PasswordResetEmail(Guid passwordResetToken)
    {
        // TODO: this.
        Body = string.Empty;
    }
}
