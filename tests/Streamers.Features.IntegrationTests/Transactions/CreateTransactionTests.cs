using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Streamers.Features.Transactions.Features.CreateTransaction;
using Streamers.Features.Transactions.Models;

namespace Streamers.Features.IntegrationTests.Transactions;

public class CreateTransactionTests : BaseIntegrationTest
{
    public CreateTransactionTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateTransaction_ValidData_CreatesTransaction()
    {
        // Arrange
        var user = await CreateStreamer(Guid.NewGuid().ToString());
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var command = new CreateTransaction(
            user.Id,
            streamer.Id,
            TransactionType.Subscription,
            10.0m,
            TransactionStatus.Succeeded,
            "http://invoice.url"
        );

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();

        var transaction = await DbContext.Transactions.FindAsync(response.Id);
        transaction.Should().NotBeNull();
        transaction!.UserId.Should().Be(user.Id);
        transaction.StreamerId.Should().Be(streamer.Id);
        transaction.GrossAmount.Should().Be(10.0m);
        transaction.Status.Should().Be(TransactionStatus.Succeeded);
        transaction.StripeInvoiceUrl.Should().Be("http://invoice.url");
    }

    [Fact]
    public async Task CreateTransaction_UserNotFound_ThrowsException()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var command = new CreateTransaction(
            "non-existent-user",
            streamer.Id,
            TransactionType.Subscription,
            10.0m,
            TransactionStatus.Succeeded,
            null
        );

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("User with ID non-existent-user not found.");
    }

    [Fact]
    public async Task CreateTransaction_StreamerNotFound_ThrowsException()
    {
        // Arrange
        var user = await CreateStreamer(Guid.NewGuid().ToString());
        var command = new CreateTransaction(
            user.Id,
            "non-existent-streamer",
            TransactionType.Subscription,
            10.0m,
            TransactionStatus.Succeeded,
            null
        );

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Streamer with ID non-existent-streamer not found.");
    }

    [Fact]
    public async Task CreateTransaction_VerifyFeeCalculation_PlatformFeeIsCorrect()
    {
        // The default FeePercent is 20, from appsettings.example.json in streamer.AppHost
        // GrossAmount = 100
        // PlatformFee = 100 * (20 / 100) = 20
        // StreamerNet = 100 - 20 = 80

        // Arrange
        var user = await CreateStreamer(Guid.NewGuid().ToString());
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var command = new CreateTransaction(
            user.Id,
            streamer.Id,
            TransactionType.Subscription,
            100.0m,
            TransactionStatus.Succeeded,
            null
        );

        // Act
        var response = await Sender.Send(command);

        // Assert
        var transaction = await DbContext.Transactions.FirstAsync(x => x.Id == response.Id);
        transaction.PlatformFee.Should().Be(5.0m);
        transaction.StreamerNet.Should().Be(95.0m);
    }
}
