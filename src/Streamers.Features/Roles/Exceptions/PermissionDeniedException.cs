namespace Streamers.Features.Roles.Exceptions;

public class PermissionDeniedException(string message) : Exception(message);