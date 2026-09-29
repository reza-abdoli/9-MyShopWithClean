// Application/Features/Payments/OnlineOrderPaymentSaga.cs
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

public class OnlineOrderPaymentSaga : Saga
{
    public Guid Id { get; set; }              // = PaymentId = Authority
    public string Status { get; set; } = "";
    public long UserId { get; set; }
    public long OrderId { get; set; }
    public int Amount { get; set; }
    public string UserMobile { get; set; } = "";

    //  شروع: قبل از رفتن کاربر به صفحه‌ی بانک 
    public static async Task<(OnlineOrderPaymentSaga, CreateOnlinePendingPaymentCommand)> Start(
        StartOnlinePaymentCommand cmd,
        IRepository<User> userRepo)
    {
        var user = await userRepo.GetEntityById(cmd.UserId)
            ?? throw new InvalidOperationException("کاربر یافت نشد.");

        var saga = new OnlineOrderPaymentSaga
        {
            Id = cmd.PaymentId,
            UserId = cmd.UserId,
            OrderId = cmd.OrderId,
            Amount = cmd.Amount,
            UserMobile = user.MobileNumber ?? "",
            Status = "AwaitingBank"
        };

        return (saga, new CreateOnlinePendingPaymentCommand(cmd.PaymentId, cmd.OrderId, cmd.UserId, cmd.Amount));
    }

    public async Task Handle(
        OnlinePaymentConfirmedEvent e,
        IRepository<Domain.Entities.Order.Order> orderRepo,
        IRepository<User> userRepo,
        IRepository<WalletPaymentAttempt> attemptRepo,
        IOrderHelper orderHelper,
        IOutboxUnitOfWork outbox)
    {
        if (Status != "AwaitingBank") return;

        var (attempt, isNew) = await EnsureAttempt(attemptRepo);

        try
        {
            var order = await orderHelper.GetOrderWithDetailsOrThrow(OrderId, UserId);
            await orderHelper.ApplyPaymentAsync(
                order, new PaymentTransaction { Id = 0 }, DateTime.UtcNow, $"ONLINE-{Id}");
            orderRepo.UpdateEntity(order);

            Status = "Completed";
            MarkCompleted();
            SetResult(attemptRepo, attempt, isNew, "Succeeded", "پرداخت با موفقیت انجام شد.");

            await outbox.PublishAsync(new SendSmsCommand
            {
                PhoneNumber = UserMobile,
                Message = $"سفارش شما با موفقیت پرداخت شد. مبلغ: {Amount:N0} تومان"
            });
            await outbox.CommitAsync();
        }
        catch (InvalidOperationException)
        {
            var user = await userRepo.GetEntityById(UserId);
            if (user != null)
            {
                user.Balance += Amount;
                userRepo.UpdateEntity(user);
            }

            Status = "Failed";
            MarkCompleted();
            SetResult(attemptRepo, attempt, isNew, "Failed",
                "ثبت سفارش ناموفق بود (احتمالاً موجودی انبار کافی نیست). مبلغ به کیف پول شما واریز شد.");

            await outbox.PublishAsync(new SendSmsCommand
            {
                PhoneNumber = UserMobile,
                Message = "خطا در ثبت سفارش. مبلغ به کیف پول شما واریز شد."
            });
            await outbox.CommitAsync();
        }
    }

    public async Task Handle(
        OnlinePaymentDeclinedEvent e,
        IRepository<WalletPaymentAttempt> attemptRepo,
        IOutboxUnitOfWork outbox)
    {
        if (Status != "AwaitingBank") return;

        var (attempt, isNew) = await EnsureAttempt(attemptRepo);

        Status = "Failed";
        MarkCompleted();
        SetResult(attemptRepo, attempt, isNew, "Failed", $"پرداخت توسط بانک تأیید نشد: {e.Reason}");

        await outbox.PublishAsync(new SendSmsCommand
        {
            PhoneNumber = UserMobile,
            Message = "پرداخت شما ناموفق بود."
        });
        await outbox.CommitAsync();
    }


    private async Task<(WalletPaymentAttempt attempt, bool isNew)> EnsureAttempt(IRepository<WalletPaymentAttempt> repo)
    {
        var attempt = await repo.GetQuery().FirstOrDefaultAsync(a => a.PaymentId == Id);
        if (attempt != null)
            return (attempt, false);

        attempt = new WalletPaymentAttempt
        {
            PaymentId = Id,
            UserId = UserId,
            OrderId = OrderId,
            Status = "Processing"
        };
        await repo.AddEntity(attempt);
        return (attempt, true);
    }

    private static void SetResult(IRepository<WalletPaymentAttempt> repo,
        WalletPaymentAttempt attempt, bool isNew, string status, string message)
    {
        attempt.Status = status;
        attempt.Message = message;

        // فقط اگه از دیتابیس خونده شده Update بزن؛ تازه‌ساخته‌شده رو نه (چون هنوز Added هست)
        if (!isNew)
            repo.UpdateEntity(attempt);
    }
}