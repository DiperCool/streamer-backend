using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Streamers.Features.GetEmail;

public record GetEmailResponse(string Email);

public record GetEmail(string UserId) : IRequest<GetEmailResponse>;

public class GetEmailHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetEmail, GetEmailResponse>
{
    public async Task<GetEmailResponse> Handle(
        GetEmail request,
        CancellationToken cancellationToken
    )
    {
        var email = await streamerDbContext
            .Streamers.Where(x => x.Id == request.UserId)
            .Select(x => x.Email)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (email == null)
        {
            throw new InvalidOperationException("Email not found");
        }
        return new GetEmailResponse(email);
    }
}
