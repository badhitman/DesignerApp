////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.telegram;
using SharedLib;

namespace Telegram.Bot.Services;

/// <summary>
/// MQ listen
/// </summary>
public static class RegisterMqListenerExtension
{
    /// <summary>
    /// RegisterMqListeners
    /// </summary>
    public static IServiceCollection TelegramBotRegisterMqListeners(this IServiceCollection services)
    {
        return services
            .RegisterMqListener<SendTextMessageTelegramReceive, SendTextMessageTelegramBotModel, MessageComplexIdsModel?>()
            .RegisterMqListener<SetWebConfigReceive, TelegramBotConfigModel, object?>()
            .RegisterMqListener<GetBotTokenReceive, object, string?>()
            .RegisterMqListener<GetBotUsernameReceive, object?, string?>()
            .RegisterMqListener<ChatsReadTelegramReceive, long[]?, ChatTelegramModelDB[]?>()
            .RegisterMqListener<MessagesSelectTelegramReceive, TPaginationRequestModel<SearchMessagesChatModel>?, TPaginationResponseModel<MessageTelegramModelDB>?>()
            .RegisterMqListener<GetFileTelegramReceive, string?, byte[]?>()
            .RegisterMqListener<SendWappiMessageReceive, EntryAltExtModel?, SendMessageResponseModel?>()
            .RegisterMqListener<ChatsFindForUserTelegramReceive, long[]?, ChatTelegramModelDB[]?>()
            .RegisterMqListener<ChatsSelectTelegramReceive, TPaginationRequestModel<string?>?, TPaginationResponseModel<ChatTelegramModelDB>?>()
            .RegisterMqListener<ForwardMessageTelegramReceive, ForwardMessageTelegramBotModel?, MessageComplexIdsModel?>()
            .RegisterMqListener<ChatTelegramReadReceive, int?, ChatTelegramModelDB?>()
            .RegisterMqListener<ErrorsForChatsSelectTelegramReceive, TPaginationRequestModel<long[]?>?, TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?>()
            ;
    }
}