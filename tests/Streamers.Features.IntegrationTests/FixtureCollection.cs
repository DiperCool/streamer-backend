using Microsoft.AspNetCore.Mvc.Testing;

namespace Streamers.Features.IntegrationTests;

[CollectionDefinition(nameof(FixtureCollection))]
public class FixtureCollection : ICollectionFixture<StreamerWebApplicationFactory> { }
