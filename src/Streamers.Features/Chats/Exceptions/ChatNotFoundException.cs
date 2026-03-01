namespace Streamers.Features.Chats.Exceptions;

public class ChatNotFoundException(Guid id) : Exception($"Chat with id '{id}' was not found.");