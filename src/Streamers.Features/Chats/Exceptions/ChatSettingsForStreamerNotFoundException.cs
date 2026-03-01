namespace Streamers.Features.Chats.Exceptions;

public class ChatSettingsForStreamerNotFoundException(string streamerId) : Exception($"Chat settings for streamer with id '{streamerId}' were not found.");