using FluentEmail.Core;
using MediatR;

namespace ProjectManagementSolution.API.Features.Emails;

public class EmailNotificationHandler : INotificationHandler<EmailNotification>
{
    private readonly IFluentEmail _fluentEmail;

    public EmailNotificationHandler(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task Handle(EmailNotification notification, CancellationToken cancellationToken)
    {
        await _fluentEmail
            .Subject(notification.Email.Subject)
            .Body(notification.Email.Body)
            .SendAsync(cancellationToken);
    }
}
