namespace Streamers.Features.Chats.Exceptions;

public class ChatSettingsNotFoundException(Guid chatId) : Exception($"Chat settings for chat with id '{chatId}' were not found.");