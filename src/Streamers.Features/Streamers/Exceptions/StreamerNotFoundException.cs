namespace Streamers.Features.Streamers.Exceptions;

public class StreamerNotFoundException(string id) : Exception($"Streamer with id '{id}' was not found.");
