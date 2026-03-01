namespace Streamers.Features.Followers.Exceptions;

public class FollowerLoopException() : Exception("You cannot follow yourself.");