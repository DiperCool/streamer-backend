using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Streamers.Exceptions;

public class RtmpOptionsNotFoundException() : NotFoundException("Rtmp options not found in configuration.");