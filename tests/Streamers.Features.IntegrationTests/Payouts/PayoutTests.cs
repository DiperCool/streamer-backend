using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Payouts.Dtos;
using Streamers.Features.Payouts.Enums;
using Streamers.Features.Payouts.Features.GetAdminPayouts;
using Streamers.Features.Payouts.Features.GetPayouts;
using Streamers.Features.Payouts.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Models;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.SystemRoles.Enums;
using Xunit;

namespace Streamers.Features.IntegrationTests.Payouts;

public class PayoutTests : BaseIntegrationTest
{
    public PayoutTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetPayouts_ShouldReturnPayoutsForCurrentUser()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        // Grant Revenue permission to the streamer for themselves
        await DbContext.Roles.AddAsync(
            new Role(
                streamer,
                RoleType.Broadcaster, // Or a more appropriate role type if available
                streamer,
                DateTime.UtcNow,
                Permissions.Revenue
            )
        );
        await DbContext.SaveChangesAsync();

        var payout1 = new Payout(
            Guid.NewGuid(),
            streamer.Id,
            "stripe_payout_1",
            100,
            "usd",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow,
            0,
            null
        );
        payout1.MakePaid();
        var payout2 = new Payout(
            Guid.NewGuid(),
            streamer.Id,
            "stripe_payout_2",
            200,
            "usd",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow,
            0,
            null
        );
        var otherStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        var payout3 = new Payout(
            Guid.NewGuid(),
            otherStreamer.Id,
            "stripe_payout_3",
            50,
            "usd",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow,
            0,
            null
        );
        payout3.MakePaid();

        await DbContext.Payouts.AddRangeAsync(payout1, payout2, payout3);
        await DbContext.SaveChangesAsync();

        var query = new GetPayouts(
            streamer.Id,
            new QueryContext<PayoutDto>(),
            new PagingArguments()
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(p => p.Id == payout1.Id);
        result.Items.Should().Contain(p => p.Id == payout2.Id);
        result.Items.Length.Should().Be(2);
    }

    [Fact]
    public async Task GetPayouts_ShouldReturnEmptyListIfNoPayouts()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        // Grant Revenue permission to the streamer for themselves
        await DbContext.Roles.AddAsync(
            new Role(streamer, RoleType.Broadcaster, streamer, DateTime.UtcNow, Permissions.Revenue)
        );
        await DbContext.SaveChangesAsync();

        var query = new GetPayouts(
            streamer.Id,
            new QueryContext<PayoutDto>(),
            new PagingArguments()
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Items.Length.Should().Be(0);
    }

    [Fact]
    public async Task GetPayouts_ShouldFailWhenUserIsNotAuthorized()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var unauthorizedUser = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(unauthorizedUser.Id);

        // Do NOT grant Revenue permission

        var query = new GetPayouts(
            streamer.Id,
            new QueryContext<PayoutDto>(),
            new PagingArguments()
        );

        // Act
        var act = async () => await Sender.Send(query);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task GetAdminPayouts_ShouldReturnAllPayoutsForAdmin()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id); // Authenticate as admin

        var streamer1 = await CreateStreamer(Guid.NewGuid().ToString());
        var payout1 = new Payout(
            Guid.NewGuid(),
            streamer1.Id,
            "stripe_payout_1",
            100,
            "usd",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow,
            0,
            null
        );
        payout1.MakePaid();
        var streamer2 = await CreateStreamer(Guid.NewGuid().ToString());
        var payout2 = new Payout(
            Guid.NewGuid(),
            streamer2.Id,
            "stripe_payout_2",
            200,
            "usd",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow,
            0,
            null
        );

        await DbContext.Payouts.AddRangeAsync(payout1, payout2);
        await DbContext.SaveChangesAsync();

        var query = new GetAdminPayouts(
            new QueryContext<PayoutDto>(),
            new PagingArguments(),
            null,
            null
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(p => p.Id == payout1.Id);
        result.Items.Should().Contain(p => p.Id == payout2.Id);
        result.Items.Length.Should().Be(2);
    }

    [Fact]
    public async Task GetAdminPayouts_ShouldFailWhenUserIsNotAdmin()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id); // Not an admin

        var query = new GetAdminPayouts(
            new QueryContext<PayoutDto>(),
            new PagingArguments(),
            null,
            null
        );

        // Act
        var act = async () => await Sender.Send(query);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
