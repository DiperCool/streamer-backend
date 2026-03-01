using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Bots.Exceptions;

public class BotNotFoundException(Guid id) : NotFoundException($"Bot with id '{id}' not found.");