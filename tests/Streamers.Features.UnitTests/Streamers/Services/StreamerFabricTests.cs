using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Streamers.Features.Files;
using Streamers.Features.Roles.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Streamers.Services;
using Xunit;
using Shared.Abstractions.Domain;

namespace Streamers.Features.UnitTests.Streamers.Services;

public class StreamerFabricTests
{
    private readonly IStreamKeyGenerator _streamKeyGenerator;
    private readonly StreamerDbContext _dbContext;
    private readonly StreamerFabric _sut;

    public StreamerFabricTests()
    {
        _streamKeyGenerator = Substitute.For<IStreamKeyGenerator>();
        var dbContextOptions = new DbContextOptions<StreamerDbContext>();
        var domainEventsDispatcher = Substitute.For<IDomainEventsDispatcher>();
        _dbContext = Substitute.For<StreamerDbContext>(dbContextOptions, domainEventsDispatcher);

        var streamerDbSet = Substitute.For<DbSet<Streamer>>();
        _dbContext.Streamers.Returns(streamerDbSet);

        var roleDbSet = Substitute.For<DbSet<Role>>();
        _dbContext.Roles.Returns(roleDbSet);
        
        _sut = new StreamerFabric(_streamKeyGenerator, _dbContext);
    }

    [Fact]
    public async Task CreateStreamer_ShouldCreateStreamerWithCorrectPropertiesAndDependencies()
    {
        // Arrange
        var id = "test-id";
        var username = "test-user";
        var email = "test@email.com";
        var created = DateTime.UtcNow;

        // Act
        var streamer = await _sut.CreateStreamer(id, username, email, created, true);

        // Assert
        streamer.Should().NotBeNull();
        streamer.Id.Should().Be(id);
        streamer.UserName.Should().Be(username);
        streamer.Email.Should().Be(email);
        streamer.CreatedAt.Should().Be(created);
        streamer.FinishedAuth.Should().BeTrue();

        streamer.Profile.Should().NotBeNull();
        streamer.Setting.Should().NotBeNull();
        streamer.Chat.Should().NotBeNull();
        streamer.NotificationSettings.Should().NotBeNull();
        streamer.VodSettings.Should().NotBeNull();
        streamer.Partner.Should().NotBeNull();
        streamer.Customer.Should().NotBeNull();
        
        await _dbContext.Streamers.Received(1).AddAsync(streamer, Arg.Any<System.Threading.CancellationToken>());
        await _dbContext.Roles.Received(1).AddAsync(Arg.Any<Role>(), Arg.Any<System.Threading.CancellationToken>());
        _streamKeyGenerator.Received(1).GenerateKey(streamer.StreamSettings);
    }
}
