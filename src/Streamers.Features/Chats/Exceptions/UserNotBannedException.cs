namespace Streamers.Features.Chats.Exceptions;

public class UserNotBannedException(string userId) : Exception($"User with id '{userId}' is not banned.");