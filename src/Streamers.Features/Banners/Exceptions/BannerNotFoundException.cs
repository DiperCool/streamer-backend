using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Banners.Exceptions;

public class BannerNotFoundException(Guid id) : NotFoundException($"Banner with id '{id}' not found.");