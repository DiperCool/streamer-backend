using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Roles.Exceptions;

public class RoleNotFoundException(Guid id) : NotFoundException($"Role with id '{id}' not found.");