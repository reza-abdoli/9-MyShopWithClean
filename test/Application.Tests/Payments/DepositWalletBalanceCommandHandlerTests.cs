// // Tests/Application.Tests/Payments/DepositWalletBalanceCommandHandlerTests.cs
// using System.Threading.Tasks;
// using Application.Features.Payments.Handlers.Commands;
// using Application.Features.Payments.Requests.Commands;
// using Application.Interfaces.Persistence;
// using Domain.Entities.Order;
// using Domain.Entities.Users;
// using KafkaContracts.Notifications;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;

// namespace Application.Tests.Payments;

// public class DepositWalletBalanceCommandHandlerTests
// {
//     private readonly Mock<IRepository<PaymentTransaction>> _paymentRepo = new();
//     private readonly Mock<IRepository<User>> _userRepo = new();
//     private readonly Mock<IOutboxUnitOfWork> _outbox = new();
//     private readonly Mock<ILogger<DepositWalletBalanceCommandHandler>> _logger = new();

//     private DepositWalletBalanceCommandHandler CreateHandler() => new(
//         _paymentRepo.Object,
//         _userRepo.Object,
//         _outbox.Object,
//         _logger.Object);

//     private static PaymentTransaction PendingDeposit(long id, long userId, int amount = 20_000) => new()
//     {
//         Id = id,
//         UserId = userId,
//         OrderId = null, // شارژ کیف پول، بدون سفارش
//         Amount = amount,
//         Status = PaymentStatus.Pending
//     };

//     [Fact]
//     public async Task Handle_ValidPendingDeposit_IncreasesBalanceAndSendsSms()
//     {
//         var user = new User { Id = 1, Balance = 10_000, MobileNumber = "09120000000" };
//         var tx = PendingDeposit(id: 200, userId: user.Id, amount: 20_000);

//         _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);
//         _userRepo.Setup(r => r.GetEntityById(user.Id)).ReturnsAsync(user);

//         var handler = CreateHandler();
//         var result = await handler.Handle(new DepositWalletBalanceCommand { PaymentTransaction = tx });

//         Assert.True(result);
//         Assert.Equal(30_000, user.Balance);
//         Assert.Equal(PaymentStatus.Success, tx.Status);
//         _outbox.Verify(o => o.PublishAsync(It.Is<SendSmsCommand>(s => s.PhoneNumber == user.MobileNumber)), Times.Once);
//         _outbox.Verify(o => o.CommitAsync(), Times.Once);
//     }

//     [Fact]
//     public async Task Handle_TransactionNotFound_ReturnsFalse()
//     {
//         _paymentRepo.Setup(r => r.GetEntityById(It.IsAny<long>())).ReturnsAsync((PaymentTransaction?)null);

//         var handler = CreateHandler();
//         var result = await handler.Handle(new DepositWalletBalanceCommand
//         {
//             PaymentTransaction = new PaymentTransaction { Id = 999 }
//         });

//         Assert.False(result);
//         _outbox.Verify(o => o.CommitAsync(), Times.Never);
//     }

//     [Fact]
//     public async Task Handle_TransactionNotPending_ReturnsFalse()
//     {
//         var tx = PendingDeposit(id: 201, userId: 1);
//         tx.Status = PaymentStatus.Success; // قبلاً پردازش شده

//         _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);

//         var handler = CreateHandler();
//         var result = await handler.Handle(new DepositWalletBalanceCommand { PaymentTransaction = tx });

//         Assert.False(result);
//     }

//     [Fact]
//     public async Task Handle_TransactionTiedToOrder_ReturnsFalse()
//     {
//         // این هندلر فقط برای شارژ کیف پول (بدون سفارش) است؛ نباید تراکنش سفارش رو پردازش کنه
//         var tx = PendingDeposit(id: 202, userId: 1);
//         tx.OrderId = 55;

//         _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);

//         var handler = CreateHandler();
//         var result = await handler.Handle(new DepositWalletBalanceCommand { PaymentTransaction = tx });

//         Assert.False(result);
//     }

//     [Fact]
//     public async Task Handle_UserNotFound_ReturnsFalse()
//     {
//         var tx = PendingDeposit(id: 203, userId: 1);

//         _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);
//         _userRepo.Setup(r => r.GetEntityById(tx.UserId)).ReturnsAsync((User?)null);

//         var handler = CreateHandler();
//         var result = await handler.Handle(new DepositWalletBalanceCommand { PaymentTransaction = tx });

//         Assert.False(result);
//     }

//     [Fact]
//     public async Task Handle_UserWithoutMobileNumber_SucceedsButDoesNotPublishSms()
//     {
//         var user = new User { Id = 1, Balance = 0, MobileNumber = null! };
//         var tx = PendingDeposit(id: 204, userId: user.Id, amount: 15_000);

//         _paymentRepo.Setup(r => r.GetEntityById(tx.Id)).ReturnsAsync(tx);
//         _userRepo.Setup(r => r.GetEntityById(user.Id)).ReturnsAsync(user);

//         var handler = CreateHandler();
//         var result = await handler.Handle(new DepositWalletBalanceCommand { PaymentTransaction = tx });

//         Assert.True(result);
//         _outbox.Verify(o => o.PublishAsync(It.IsAny<SendSmsCommand>()), Times.Never);
//         _outbox.Verify(o => o.CommitAsync(), Times.Once);
//     }
// }