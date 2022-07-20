namespace ProjectManagementSolution.API.Features.Users;

public class User
{
    public Guid Id { get; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public bool EmailVerified { get; private set; } = false;
    public string ProfilePicture { get; set; } = string.Empty;
    public List<EmailVerificationToken> EmailVerificationTokens { get; private set; } = new List<EmailVerificationToken>();

    public User(string username, string passwordHash, string emailAddress)
    {
        Id = Guid.NewGuid();
        Username = username;
        PasswordHash = passwordHash;
        EmailAddress = emailAddress;
    }

    public EmailVerificationToken CreateEmailVerificationToken()
    {
        var expiresAt = DateTime.UtcNow.AddDays(30);
        var emailVerificationToken = new EmailVerificationToken(expiresAt, this);
        return emailVerificationToken;
    }

    public void Verify()
    {
        EmailVerified = true;
    }
}
