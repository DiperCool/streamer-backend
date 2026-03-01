namespace Streamers.Features.Chats.Exceptions;

public class MessageNotFoundException(Guid id) : Exception($"Message with id '{id}' was not found.");