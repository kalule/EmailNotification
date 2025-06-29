﻿using Common.Events.Interfaces;
using Common.Events.Models;
using Common.Messaging.Interfaces;
using Common.Messaging.Models;

namespace EmailNotification.Models
{
    public class EmailRequestedEventHandler
    : BaseEventHandler<EmailRequestedEvent>, IEventBusIntegrationEventHandler<EmailRequestedEvent>
    {
        private readonly ILogger<EmailRequestedEventHandler> _logger;
        private readonly IEmailSender _emailSender;

        public EmailRequestedEventHandler(ILogger<EmailRequestedEventHandler> logger, IEmailSender emailSender) : base(logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }
        protected override async Task<bool> ProcessEventsAsync(Common.Events.Models.EmailRequestedEvent eventData)
        {
            if (eventData == null)
            {
                _logger.LogWarning("Received null EmailRequestedEvent.");
                return false;
            }

            _logger.LogInformation("Sending email to: {Email}", eventData.Recipients);

            if (!eventData.Recipients.Any())
            {
                _logger.LogWarning("Invalid email event data: To or Subject is missing.");
                return false;
            }

            // Map attachments from eventData to EmailRequest model
            var attachmentList = eventData.Attachments?.Select(a => new Common.Messaging.Models.EmailAttachment
                {
                    FileName = a.FileName,
                    FileBase64Content = a.FileBase64Content,
                    ContentType = a.ContentType
                }).ToList() ?? new();

            var recipients = eventData.Recipients
                            .Select(c => c.Trim())
                            .Where(c => !string.IsNullOrWhiteSpace(c))
                            .ToList();

            var request = new EmailRequest
                {
                    Recipients = recipients,
                    Subject = eventData.Subject,
                    BodyHtml = eventData.BodyHtml ?? string.Empty,
                    Attachments = attachmentList
                };

            try
            {
                await _emailSender.SendAsync(request);
                _logger.LogInformation("Email sent to {Email}", string.Join(", " , recipients));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", string.Join(", ", recipients));
                return false;
            }
        }
    }
}


