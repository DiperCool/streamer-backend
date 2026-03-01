using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Streams.Exceptions;

public class StreamNotFoundException(string streamerId) : NotFoundException($"Stream for streamer with id '{streamerId}' not found.");