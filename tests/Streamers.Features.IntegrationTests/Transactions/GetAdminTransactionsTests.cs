using System;
using System.Threading.Tasks;
using FluentAssertions;
using GreenDonut.Data;
using Streamers.Features.Transactions.Dtos;
using Streamers.Features.Transactions.Features.GetAdminTransactions;
using Streamers.Features.Transactions.Models;

namespace Streamers.Features.IntegrationTests.Transactions;

public class GetAdminTransactionsTests : BaseIntegrationTest
{
    public GetAdminTransactionsTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetAdminTransactions_UserIsAdmin_ReturnsAllTransactions()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);

        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var user = await CreateStreamer(Guid.NewGuid().ToString());

        var transaction1 = new Transaction(
            Guid.NewGuid(),
            user.Id,
            user,
            streamer.Id,
            streamer,
            TransactionType.Subscription,
            10,
            1,
            9,
            TransactionStatus.Succeeded,
            DateTime.UtcNow,
            null
        );
        var transaction2 = new Transaction(
            Guid.NewGuid(),
            user.Id,
            user,
            streamer.Id,
            streamer,
            TransactionType.Subscription,
            20,
            2,
            18,
            TransactionStatus.Succeeded,
            DateTime.UtcNow,
            null
        );

        await DbContext.Transactions.AddRangeAsync(transaction1, transaction2);
        await DbContext.SaveChangesAsync();

        var query = new GetAdminTransactions(
            new QueryContext<TransactionDto>(),
            new PagingArguments(),
            null,
            null
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAdminTransactions_UserIsNotAdmin_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(user.Id);

        var query = new GetAdminTransactions(
            new QueryContext<TransactionDto>(),
            new PagingArguments(),
            null,
            null
        );

        // Act
        var act = async () => await Sender.Send(query);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetAdminTransactions_WithDateFilters_ReturnsFilteredTransactions()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);

        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var user = await CreateStreamer(Guid.NewGuid().ToString());

        var transaction1 = new Transaction(
            Guid.NewGuid(),
            user.Id,
            user,
            streamer.Id,
            streamer,
            TransactionType.Subscription,
            10,
            1,
            9,
            TransactionStatus.Succeeded,
            DateTime.UtcNow.AddDays(-2),
            null
        );
        var transaction2 = new Transaction(
            Guid.NewGuid(),
            user.Id,
            user,
            streamer.Id,
            streamer,
            TransactionType.Subscription,
            20,
            2,
            18,
            TransactionStatus.Succeeded,
            DateTime.UtcNow,
            null
        );

        await DbContext.Transactions.AddRangeAsync(transaction1, transaction2);
        await DbContext.SaveChangesAsync();

        var query = new GetAdminTransactions(
            new QueryContext<TransactionDto>(),
            new PagingArguments(),
            DateTime.UtcNow.AddDays(-1),
            null
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.Should().Contain(t => t.Id == transaction2.Id);
    }

    [Fact]
    public async Task GetAdminTransactions_NoTransactions_ReturnsEmptyPage()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);

        var query = new GetAdminTransactions(
            new QueryContext<TransactionDto>(),
            new PagingArguments(),
            null,
            null
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }
}
