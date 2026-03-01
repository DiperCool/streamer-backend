using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Vods.Exceptions;

public class VodSettingsNotFoundException() : NotFoundException("No vod settings found.");