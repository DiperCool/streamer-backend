using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Streamers.Exceptions;

public class StreamerAlreadyExistsException(string id) : Exception($"Streamer with id '{id}' already exists.");