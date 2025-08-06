using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Streamers.Dtos;

namespace Streamers.Features.Streamers.GraphqQl;

[ObjectType<StreamerDto>]
public static partial class StreamerType { }
