using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Vods.Exceptions;

public class VodNotFoundException(Guid id) : NotFoundException($"Vod with id '{id}' not found.");