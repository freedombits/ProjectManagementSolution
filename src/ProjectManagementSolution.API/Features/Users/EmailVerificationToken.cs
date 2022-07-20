namespace ProjectManagementSolution.API.Features.Users;

public class EmailVerificationToken
{
    public Guid Token { get; }
    public DateTime Expires { get; }

    public User User { get; }
    public Guid UserId { get; }

    public bool Active { get; private set; }

    public EmailVerificationToken(DateTime expires, User user)
    {
        Token = Guid.NewGuid();
        Expires = expires;
        User = user;
        UserId = user.Id;
        Active = true;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > Expires;
    }

    public void Deactivate()
    {
        Active = false;
    }
}
