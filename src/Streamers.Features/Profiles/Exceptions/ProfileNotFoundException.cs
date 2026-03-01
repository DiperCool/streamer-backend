using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Profiles.Exceptions;

public class ProfileNotFoundException(string streamerId) : NotFoundException($"Profile for user with id '{streamerId}' not found.");