using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Streams.Exceptions;

public class StreamSettingsNotFoundException(string streamerId) : NotFoundException($"Stream settings for streamer with id '{streamerId}' not found.");