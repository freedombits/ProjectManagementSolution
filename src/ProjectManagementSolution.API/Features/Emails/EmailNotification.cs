using MediatR;

namespace ProjectManagementSolution.API.Features.Emails;

public record EmailNotification(IEmail Email) : INotification;
