namespace Streamers.Features.Chats.Exceptions;

public class UserAlreadyBannedException(string userId) : Exception($"User with id '{userId}' is already banned.");