namespace Streamers.Features.Shared.Exceptions;

public class ForbiddenException(string message = "You are not authorized to perform this action.") : Exception(message);
