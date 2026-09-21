// test/Application.Tests/Payments/PayOrderWithWalletCommandHandlerTests.cs
using Application.Features.Orders;
using Application.Features.Payments.Handlers.Commands;
using Application.Features.Payments.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using KafkaContracts.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;

// نام Order و Product هم namespace هستن هم کلاس؛ با alias تداخل رو حل می‌کنیم
using OrderEntity = Domain.Entities.Order.Order;
using ProductEntity = Domain.Entities.Product.Product;
using ProductVariantEntity = Domain.Entities.Product.ProductVariant;

namespace Application.Tests.Payments;

public class PayOrderWithWalletCommandHandlerTests
{
    private const long UserId = 1;
    private const long OrderId = 10;
    private const string Mobile = "09120000000";

    // ---- وابستگی‌های جعلی ----
    private readonly Mock<IRepository<User>> _userRepo = new();
    private readonly Mock<IRepository<OrderEntity>> _orderRepo = new();
    private readonly Mock<IRepository<PaymentTransaction>> _paymentRepo = new();
    private readonly Mock<IOrderHelper> _orderHelper = new();
    private readonly Mock<IOutboxUnitOfWork> _outbox = new();
    private readonly Mock<ILogger<PayOrderWithWalletCommandHandler>> _logger = new();

    private readonly PayOrderWithWalletCommand _command = new()
    {
        UserId = UserId,
        OrderId = OrderId,
        TotalCartPrice = 100_000
    };

    private PayOrderWithWalletCommandHandler CreateSut() => new(
        _userRepo.Object,
        _orderRepo.Object,
        _paymentRepo.Object,
        _orderHelper.Object,
        _outbox.Object,
        _logger.Object);
    // = new PayOrderWithWalletCommandHandler(_userRepo.Object,_orderRepo.Object,...)
    
    // ---- ساخت داده‌ی تست ----
    private static User MakeUser(int balance = 500_000, string? mobile = Mobile) => new()
    {
        Id = UserId,
        Balance = balance,
        MobileNumber = mobile
    };

    // یک آیتم با قیمت ۵۰٬۰۰۰ و تعداد ۲  => total = 100٬000
    private static OrderEntity MakeOrder(bool withItems = true) => new()
    {
        Id = OrderId,
        UserId = UserId,
        OrderDetails = withItems
            ? new List<OrderDetail>
            {
                new()
                {
                    Count = 2,
                    ProductVariant = new ProductVariantEntity
                    {
                        Product = new ProductEntity { Price = 50_000 }
                    }
                }
            }
            : new List<OrderDetail>()
    };

    // حالت «همه چیز درسته». هر تست فقط چیزی که می‌خواد رو خراب می‌کنه.
    private void SetupHappyPath(User? user = null, OrderEntity? order = null, PaymentTransaction? existingTx = null)
    {
        _userRepo.Setup(r => r.GetEntityById(UserId)).ReturnsAsync(user ?? MakeUser());

        _orderHelper
            .Setup(h => h.GetOrderWithDetailsOrThrow(OrderId, UserId))
            .ReturnsAsync(order ?? MakeOrder());

        var txList = existingTx == null
            ? new List<PaymentTransaction>()
            : new List<PaymentTransaction> { existingTx };
        // _paymentRepo.Setup(r => r.GetQuery()).Returns(txList.BuildMock());
        _paymentRepo.Setup(r => r.GetQuery()).Returns(txList.BuildMockDbSet().Object);
    }

    // ------------------------------------------------------------------
    [Fact]
    public async Task Handle_WhenPaymentIsValid_DeductsBalanceSavesAndSendsSms()
    {
        var user = MakeUser(balance: 500_000);
        SetupHappyPath(user: user);

        var result = await CreateSut().Handle(_command);

        Assert.True(result);
        Assert.Equal(400_000, user.Balance); // 500k - 100k

        _paymentRepo.Verify(r => r.AddEntity(It.Is<PaymentTransaction>(t =>
            t.Amount == 100_000 &&
            t.Status == PaymentStatus.Success &&
            t.Authority.StartsWith("WALLET_"))), Times.Once);

        _orderHelper.Verify(h => h.ApplyPaymentAsync(
            It.IsAny<OrderEntity>(), It.IsAny<PaymentTransaction>(), It.IsAny<DateTime>()), Times.Once);

        _outbox.Verify(o => o.PublishAsync(It.Is<SendSmsCommand>(m => m.PhoneNumber == Mobile)), Times.Once);
        _outbox.Verify(o => o.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        // when we use It.Is<Something>(x => x.Y == ...) we have to specify clauses for "Something" exp: _outbox.Verify(o => o.PublishAsync(It.IsAny<SendSmsCommand>()), Times.Once);
        // but in It.IsAny we dont have to
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFalseAndCommitsNothing()
    {
        _userRepo.Setup(r => r.GetEntityById(UserId)).ReturnsAsync((User?)null);

        var result = await CreateSut().Handle(_command);

        Assert.False(result);
        _outbox.Verify(o => o.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCartIsEmpty_ReturnsFalse()
    {
        SetupHappyPath(order: MakeOrder(withItems: false));

        var result = await CreateSut().Handle(_command);

        Assert.False(result);
        _outbox.Verify(o => o.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBalanceIsInsufficient_ReturnsFalseAndChangesNothing()
    {
        var user = MakeUser(balance: 50_000); // کمتر از ۱۰۰٬۰۰۰
        SetupHappyPath(user: user);

        var result = await CreateSut().Handle(_command);

        Assert.False(result);
        Assert.Equal(50_000, user.Balance); // دست نخورده

        _paymentRepo.Verify(r => r.AddEntity(It.IsAny<PaymentTransaction>()), Times.Never);
        _paymentRepo.Verify(r => r.UpdateEntity(It.IsAny<PaymentTransaction>()), Times.Never);
        _userRepo.Verify(r => r.UpdateEntity(It.IsAny<User>()), Times.Never);
        _outbox.Verify(o => o.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTransactionAlreadyExists_UpdatesInsteadOfAdding()
    {
        var existing = new PaymentTransaction
        {
            Id = 5,
            UserId = UserId,
            OrderId = OrderId,
            Status = PaymentStatus.Failed
        };
        SetupHappyPath(existingTx: existing);

        var result = await CreateSut().Handle(_command);

        Assert.True(result);
        _paymentRepo.Verify(r => r.UpdateEntity(existing), Times.Once);
        _paymentRepo.Verify(r => r.AddEntity(It.IsAny<PaymentTransaction>()), Times.Never);
        Assert.Equal(PaymentStatus.Success, existing.Status);
    }

    [Fact]
    public async Task Handle_WhenConcurrencyExceptionOccurs_ReturnsFalse()
    {
        SetupHappyPath();
        _outbox.Setup(o => o.CommitAsync(It.IsAny<CancellationToken>()))
               .ThrowsAsync(new DbUpdateConcurrencyException());

        var result = await CreateSut().Handle(_command);

        Assert.False(result);
    }
}