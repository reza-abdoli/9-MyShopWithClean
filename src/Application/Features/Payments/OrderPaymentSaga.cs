// Application/Features/Payments/OrderPaymentSaga.cs
using System;
using System.Threading.Tasks;
using KafkaContracts.Notifications;
using KafkaContracts.Payments;
using Application.Interfaces.Persistence;
using Application.Features.Orders;
using Application.Features.Payments.Requests.Commands;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Application.Features.Payments;

public class OrderPaymentSaga : Saga
{
    public Guid Id { get; set; }              // = PaymentId
    public string Status { get; set; } = "";
    public long UserId { get; set; }
    public long OrderId { get; set; }
    public int Amount { get; set; }
    public string UserMobile { get; set; } = "";

    // ---- شروع: کسر کیف‌پول + ثبت «در حال پردازش» ----
    public static async Task<(OrderPaymentSaga, RegisterPaymentCommand, SendSmsCommand)> Start(
        StartWalletPaymentCommand cmd,
        IRepository<User> userRepo,
        IRepository<WalletPaymentAttempt> attemptRepo)
    {
        var user = await userRepo.GetEntityById(cmd.UserId)
            ?? throw new InvalidOperationException("کاربر یافت نشد.");

        if (user.Balance < cmd.Amount)
            throw new InvalidOperationException("موجودی کیف پول کافی نیست.");

        user.Balance -= cmd.Amount;
        userRepo.UpdateEntity(user);

        await attemptRepo.AddEntity(new WalletPaymentAttempt
        {
            PaymentId = cmd.PaymentId,
            UserId = cmd.UserId,
            OrderId = cmd.OrderId,
            Status = "Processing"
        });

        var saga = new OrderPaymentSaga
        {
            Id = cmd.PaymentId,
            UserId = cmd.UserId,
            OrderId = cmd.OrderId,
            Amount = cmd.Amount,
            UserMobile = user.MobileNumber ?? "",
            Status = "WaitingForRegistration"
        };

        return (saga, new RegisterPaymentCommand(cmd.PaymentId, cmd.OrderId, cmd.UserId, cmd.Amount), new SendSmsCommand {  PhoneNumber = user.MobileNumber, Message = $"سفارش شما با موفقیت ثبت شد. کد {cmd.PaymentId} مبلغ: {cmd.Amount:N0} تومان"});
    }

    // ---- ثبت موفق شد → انبار و سفارش ----
    public async Task Handle(
        PaymentRegisteredEvent e,
        IRepository<Domain.Entities.Order.Order> orderRepo,
        IRepository<WalletPaymentAttempt> attemptRepo,
        IOrderHelper orderHelper,
        IOutboxUnitOfWork outbox)
    {
        if (Status != "WaitingForRegistration") return;   // پیام تکراری

        try
        {
            var order = await orderHelper.GetOrderWithDetailsOrThrow(OrderId, UserId);
            await orderHelper.ApplyPaymentAsync(
                order, new PaymentTransaction { Id = 0 }, DateTime.UtcNow, $"WALLET-{Id}");
            orderRepo.UpdateEntity(order);
        }
        catch (InvalidOperationException)
        {
            Status = "Compensating";
            await outbox.PublishAsync(new VoidPaymentCommand(Id));
            await outbox.CommitAsync();
            return;
        }

        Status = "Completed";
        MarkCompleted();

        var attempt = await GetAttempt(attemptRepo);
        SetResult(attemptRepo, attempt, "Succeeded", "پرداخت با موفقیت انجام شد.");

        await outbox.PublishAsync(new SendSmsCommand
        {
            PhoneNumber = UserMobile,
            Message = $"سفارش شما با موفقیت پرداخت شد. مبلغ: {Amount:N0} تومان"
        });
        await outbox.CommitAsync();
    }

    // ---- ثبت رد شد (Rollback محلی) ----
    public async Task Handle(
        PaymentRegistrationFailedEvent e,
        IRepository<User> userRepo,
        IRepository<WalletPaymentAttempt> attemptRepo,
        IOutboxUnitOfWork outbox)
    {
        if (Status != "WaitingForRegistration") return;

        await RefundWalletLocally(userRepo);
        Status = "Failed";
        MarkCompleted();

        var attempt = await GetAttempt(attemptRepo);
        SetResult(attemptRepo, attempt, "Failed",
            $"پرداخت رد شد: {e.Reason}. مبلغ به کیف پول شما برگردانده شد.");

        await outbox.PublishAsync(new SendSmsCommand
        {
            PhoneNumber = UserMobile,
            Message = "پرداخت شما رد شد."
        });
        await outbox.CommitAsync();
    }

    // ---- Void تأیید شد (Rollback بین‌سرویسی) ----
    public async Task Handle(
        PaymentVoidedEvent e,
        IRepository<User> userRepo,
        IRepository<WalletPaymentAttempt> attemptRepo,
        IOutboxUnitOfWork outbox)
    {
        if (Status != "Compensating") return;

        await RefundWalletLocally(userRepo);
        Status = "Failed";
        MarkCompleted();

        var attempt = await GetAttempt(attemptRepo);
        SetResult(attemptRepo, attempt, "Failed",
            "ثبت سفارش ناموفق بود (احتمالاً موجودی انبار کافی نیست). مبلغ به کیف پول شما برگردانده شد.");

        await outbox.PublishAsync(new SendSmsCommand
        {
            PhoneNumber = UserMobile,
            Message = "خطا در ثبت سفارش. مبلغ به کیف پول شما برگردانده شد."
        });
        await outbox.CommitAsync();
    }

    private Task<WalletPaymentAttempt?> GetAttempt(IRepository<WalletPaymentAttempt> repo)
        => repo.GetQuery().FirstOrDefaultAsync(a => a.PaymentId == Id);

    private static void SetResult(IRepository<WalletPaymentAttempt> repo,
        WalletPaymentAttempt? attempt, string status, string? message)
    {
        if (attempt == null) return;
        attempt.Status = status;
        attempt.Message = message;
        repo.UpdateEntity(attempt);
    }

    private async Task RefundWalletLocally(IRepository<User> userRepo)
    {
        var user = await userRepo.GetEntityById(UserId);
        if (user != null)
        {
            user.Balance += Amount;
            userRepo.UpdateEntity(user);
        }
    }
}