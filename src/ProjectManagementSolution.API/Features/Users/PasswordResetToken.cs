namespace ProjectManagementSolution.API.Features.Users;

public class PasswordResetToken
{
    public Guid Id { get; }
    public bool Active { get; private set; }
    public DateTime Expires { get; }


    public User User { get; }
    public Guid UserId { get; }

    public PasswordResetToken(DateTime expiresAt, User user)
    {
        Id = Guid.NewGuid();
        Active = true;
        Expires = expiresAt;
        User = user;
        UserId = user.Id;
        Expires = DateTime.UtcNow;
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
