using FluentEmail.Core;
using MediatR;

namespace ProjectManagementSolution.API.Features.Emails;

public class EmailNotificationHandler : INotificationHandler<EmailNotification>
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<EmailNotificationHandler> _logger;

    public EmailNotificationHandler(
        IFluentEmail fluentEmail,
        ILogger<EmailNotificationHandler> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public async Task Handle(EmailNotification notification, CancellationToken cancellationToken)
    {
        var sendResponse = await _fluentEmail
            .Subject(notification.Email.Subject)
            .Body(notification.Email.Body)
            .SendAsync(cancellationToken);

        if (sendResponse.ErrorMessages.Any())
        {
            // TODO: I'm not sure if this is correct.
            _logger.LogInformation(
                "Failed to send email to {User}. Errors: {Errors}",
                notification.Email.Subject,
                sendResponse.ErrorMessages);
        }
    }
}
