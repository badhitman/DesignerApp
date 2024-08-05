////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Сервис отправки Email
/// </summary>
public interface IMailProviderService
{
    /// <summary>
    /// Отправка Email
    /// </summary>
    /// <param name="email">Получатель</param>
    /// <param name="subject">Тема письма</param>
    /// <param name="message">Сообщение</param>
    /// <param name="mimekit_format">Формат сообщения (MimeKit: Plain, Plain, Flowed, Html, Enriched, CompressedRichText, RichText)</param>
    public Task<ResponseBaseModel> SendEmailAsync(string email, string subject, string message, string mimekit_format = "html");

    /// <summary>
    /// Разослать техническое уведомления получателям из [SmtpConfigModel.EmailNotificationRecipients]
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="mimekit_format">Формат сообщения (MimeKit: Plain, Plain, Flowed, Html, Enriched, CompressedRichText, RichText)</param>
    public Task<ResponseBaseModel> SendTechnicalEmailNotificationAsync(string message, string mimekit_format = "html");
}