namespace Streamers.Features.Chats.Exceptions;

public class ChatForStreamerNotFoundException(string streamerId) : Exception($"Chat for streamer with id '{streamerId}' was not found.");