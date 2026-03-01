using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Partners.Exceptions;

public class PartnerNotFoundException(string streamerId) : NotFoundException($"Partner for streamer with id '{streamerId}' not found.");