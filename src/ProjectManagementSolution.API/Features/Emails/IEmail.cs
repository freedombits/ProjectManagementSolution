namespace ProjectManagementSolution.API.Features.Emails;

public interface IEmail
{
    string Subject { get; }
    string Body { get; }
}
