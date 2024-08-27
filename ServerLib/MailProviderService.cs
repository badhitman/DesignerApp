////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Сервис отправки Email
/// </summary>
public class MailProviderService(IOptions<SmtpConfigModel> _config, ILogger<MailProviderService> loggerRepo) : IMailProviderService
{
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SendEmailAsync(string email, string subject, string message, string mimekit_format = "html")
    {
        if (!System.Net.Mail.MailAddress.TryCreate(email, out System.Net.Mail.MailAddress? mail) || mail is null)
            return ResponseBaseModel.CreateError("Не корректный Email");

        if (mail.Host == GlobalStaticConstants.FakeHost)
            return ResponseBaseModel.CreateInfo("Заглушка: email host");

        TextFormat format = (TextFormat)Enum.Parse(typeof(TextFormat), mimekit_format, true);
        MimeMessage? emailMessage = new();

        emailMessage.From.Add(new MailboxAddress(_config.Value.PublicName, _config.Value.Email));
        emailMessage.To.Add(new MailboxAddress(string.Empty, email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(format)
        {
            Text = message
        };

        return await SendMessageAsync(emailMessage);
    }

    async Task<ResponseBaseModel> SendMessageAsync(MimeMessage emailMessage)
    {
        using SmtpClient? client = new();
        try
        {
            await client.ConnectAsync(_config.Value.Host, _config.Value.Port, _config.Value.UseSsl);
            await client.AuthenticateAsync(_config.Value.Login, _config.Value.Password);
        }
        catch (Exception ex)
        {
            loggerRepo.LogError(ex, "error 844D231A-1CD4-470F-AF56-833AAA86BDEA");
            return ResponseBaseModel.CreateError(ex);
        }

        string? res;
        try
        {
            res = await client.SendAsync(emailMessage);
        }
        catch (Exception ex)
        {
            return ResponseBaseModel.CreateError(ex);
        }

        await client.DisconnectAsync(true);
        return ResponseBaseModel.CreateSuccess(res);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SendTechnicalEmailNotificationAsync(string message, string mimekit_format = "html")
    {
        TextFormat format = (TextFormat)Enum.Parse(typeof(TextFormat), mimekit_format, true);
        MimeMessage? emailMessage = new();

        emailMessage.From.Add(new MailboxAddress(_config.Value.PublicName, _config.Value.Email));
        emailMessage.To.AddRange(_config.Value.EmailNotificationRecipients.DistinctBy(x => x.ToLower()).Select(x => new MailboxAddress(string.Empty, x)));
        emailMessage.Subject = "ВАЖНОЕ! Серверное уведомление.";
        emailMessage.XPriority = XMessagePriority.High;
        emailMessage.Body = new TextPart(format)
        {
            Text = message
        };

        return await SendMessageAsync(emailMessage);
    }
}