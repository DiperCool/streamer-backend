using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Subscriptions.Exceptions;

public class SubscriptionPlanNotFoundException() : NotFoundException("Subscription plan not found.");