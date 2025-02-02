using Dotnet.Homeworks.Mailing.API.Configuration;
using Dotnet.Homeworks.Mailing.API.Dto;
using Dotnet.Homeworks.Shared.Dto;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dotnet.Homeworks.Mailing.API.Services;

public class MailingService : IMailingService
{
    private readonly EmailConfig _emailConfig;
    private readonly ILogger<MailingService> _logger;

    public MailingService(IOptions<EmailConfig> emailConfig, ILogger<MailingService> logger)
    {
        _logger = logger;
        _emailConfig = emailConfig.Value;
    }

    public async Task<Result> SendEmailAsync(EmailMessage emailDto)
    {
        await Task.Delay(5000);
        
        _logger.LogInformation($"From Email: {_emailConfig.Email}; From Password: {_emailConfig.Password};" +
                               $"From Host: {_emailConfig.Host}; From Port: {_emailConfig.Port};" +
                               $"To Email: {emailDto.Email}; Subject: {emailDto.Subject} Message: {emailDto.Content}");

        return new Result(true);
    }
}