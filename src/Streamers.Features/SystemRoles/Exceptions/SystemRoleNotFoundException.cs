using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.SystemRoles.Exceptions;

public class SystemRoleNotFoundException(string userId) : NotFoundException($"System role for user with id '{userId}' not found.");