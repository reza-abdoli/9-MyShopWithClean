// test/Application.Tests/Payments/FinalizeOnlineOrderPaymentCommandHandlerTests.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Orders;
using Application.Features.Payments;
using Application.Features.Payments.Handlers.Commands;
using Application.Features.Payments.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using KafkaContracts.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

// نام Order هم namespace هست هم کلاس؛ با alias تداخل رو حل می‌کنیم (دقیقاً مثل تست اول)
using OrderEntity = Domain.Entities.Order.Order;

namespace Application.Tests.Payments;

public class FinalizeOnlineOrderPaymentCommandHandlerTests
{
    private readonly Mock<IRepository<PaymentTransaction>> _paymentRepo = new();
    private readonly Mock<IRepository<OrderEntity>> _orderRepo = new();
    private readonly Mock<IRepository<User>> _userRepo = new();
    private readonly Mock<IOrderHelper> _orderHelper = new();
    private readonly Mock<IOutboxUnitOfWork> _outbox = new();
    private readonly Mock<IServiceScopeFactory> _scopeFactory = new();
    private readonly Mock<ILogger<FinalizeOnlineOrderPaymentCommandHandler>> _logger = new();

    private FinalizeOnlineOrderPaymentCommandHandler CreateSut() => new(
        _paymentRepo.Object,
        _orderRepo.Object,
        _userRepo.Object,
        _orderHelper.Object,
        _outbox.Object,
        _scopeFactory.Object,
        _logger.Object);

    private static PaymentTransaction PendingTx(long id, long userId, long orderId, int amount = 50_000) => new()
    {
        Id = id,
        UserId = userId,
        OrderId = orderId,
        Amount = amount,
        Status = PaymentStatus.Pending
    };

    // scope + IOnlinePaymentRefunder جعلی رو با هم می‌سازه تا توی هر تست تکرار نشه
    private Mock<IOnlinePaymentRefunder> SetupRefunder()
    {
        var refunder = new Mock<IOnlinePaymentRefunder>();

        var scope = new Mock<IServiceScope>();
        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(IOnlinePaymentRefunder))).Returns(refunder.Object);
        scope.Setup(s => s.ServiceProvider).Returns(provider.Object);

        _scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);

        return refunder;
    }

    [Fact]
    public async Task Handle_ValidPendingTransaction_FinalizesAndSendsSms()
    {
        var user = new User { Id = 1, MobileNumber = "09120000000" };
        var tx = PendingTx(id: 100, userId: user.Id, orderId: 10);
        var order = new OrderEntity { Id = 10, UserId = user.Id };

        _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);
        _orderHelper.Setup(h => h.GetOrderWithDetailsOrThrow(tx.OrderId!.Value, tx.UserId)).ReturnsAsync(order);
        _userRepo.Setup(r => r.GetEntityById(user.Id)).ReturnsAsync(user);

        var result = await CreateSut().Handle(new FinalizeOnlineOrderPaymentCommand { PaymentTransaction = tx });

        Assert.True(result);
        Assert.Equal(PaymentStatus.Success, tx.Status);
        _outbox.Verify(o => o.PublishAsync(It.Is<SendSmsCommand>(s => s.PhoneNumber == user.MobileNumber)), Times.Once);
        _outbox.Verify(o => o.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _scopeFactory.Verify(s => s.CreateScope(), Times.Never);
    }

    [Fact]
    public async Task Handle_TransactionNotPending_ReturnsFalseWithoutTouchingAnything()
    {
        var tx = PendingTx(id: 101, userId: 1, orderId: 10);
        tx.Status = PaymentStatus.Success; 

        _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);

        var result = await CreateSut().Handle(new FinalizeOnlineOrderPaymentCommand { PaymentTransaction = tx });

        Assert.False(result);
        _orderHelper.Verify(h => h.GetOrderWithDetailsOrThrow(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        _outbox.Verify(o => o.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TransactionNotFound_ReturnsFalse()
    {
        _paymentRepo.Setup(r => r.GetEntityById(It.IsAny<long>())).ReturnsAsync((PaymentTransaction?)null);

        var result = await CreateSut().Handle(new FinalizeOnlineOrderPaymentCommand
        {
            PaymentTransaction = new PaymentTransaction { Id = 999 }
        });

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ExceptionDuringFinalize_TriggersRefundAndReturnsFalse()
    {
        var tx = PendingTx(id: 102, userId: 1, orderId: 10);

        _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);
        _orderHelper
            .Setup(h => h.GetOrderWithDetailsOrThrow(tx.OrderId!.Value, tx.UserId))
            .ThrowsAsync(new InvalidOperationException("order mismatch"));

        var refunder = SetupRefunder();
        refunder.Setup(r => r.RefundToWalletAsync(tx.Id)).ReturnsAsync(true);

        var result = await CreateSut().Handle(new FinalizeOnlineOrderPaymentCommand { PaymentTransaction = tx });

        Assert.False(result);
        refunder.Verify(r => r.RefundToWalletAsync(tx.Id), Times.Once);
        _scopeFactory.Verify(f => f.CreateScope(), Times.Once); 
    }

    [Fact]
    public async Task Handle_FinalizeFailsAndRefundFailsAllAttempts_LogsCriticalAndReturnsFalse()
    {
        var tx = PendingTx(id: 103, userId: 1, orderId: 10);

        _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);
        _orderHelper
            .Setup(h => h.GetOrderWithDetailsOrThrow(tx.OrderId!.Value, tx.UserId))
            .ThrowsAsync(new InvalidOperationException("order mismatch"));

        var refunder = SetupRefunder();
        refunder.Setup(r => r.RefundToWalletAsync(tx.Id)).ThrowsAsync(new Exception("refund failed"));

        var result = await CreateSut().Handle(new FinalizeOnlineOrderPaymentCommand { PaymentTransaction = tx });

        Assert.False(result);
        _scopeFactory.Verify(f => f.CreateScope(), Times.Exactly(3)); // MaxRefundAttempts
        _logger.Verify(l => l.Log(
            LogLevel.Critical,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}