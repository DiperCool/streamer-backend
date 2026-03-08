namespace Streamers.Features.IntegrationTests;

[CollectionDefinition(nameof(FixtureCollection), DisableParallelization = true)]
public class FixtureCollection : ICollectionFixture<StreamerWebApplicationFactory> { }
