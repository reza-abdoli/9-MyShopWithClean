// Api/Controllers/UserControllers/PaymentController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Account;
using Application.Features.Orders.Requests.Queries;
using Application.Features.Payments.Requests.Commands;
using Application.Features.Payments.Requests.Queries;
using Application.Features.Payments.Responses;
using Application.Features.Users.Requests.Queries;
using KafkaContracts.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.UserControllers;

[Authorize]
[Route("api/payments")]
public class PaymentController : ApiControllerBase
{
    private const int MaxOrderAmount = 100_000_000;

    [HttpPost("pay-order")]
    public async Task<ActionResult<PayOrderResponseDto>> PayOrder(bool isWalletPayment)
    {
        var openOrder = await Bus.InvokeAsync<Application.DTOs.Order.OpenOrderDto?>(new UserOpenOrderDetailQuery { UserId = UserId });

        if (openOrder == null)
            return BadRequest(new { message = "سبد خرید شما خالی است." });

        if (openOrder.TotalCartPrice() > MaxOrderAmount)
            return BadRequest(new { message = "سفارش‌های بالای ۱۰۰ میلیون تومان قابل پرداخت نیستند." });

        if (isWalletPayment)
        {
            var balance = await Bus.InvokeAsync<int>(new GetUserBalanceQuery { UserId = UserId });
            if (openOrder.TotalCartPrice() > balance)
                return BadRequest(new { message = "موجودی کیف پول شما کافی نیست." });

            var walletPaymentId = Guid.NewGuid();

            try
            {
                await Bus.InvokeAsync(new StartWalletPaymentCommand
                {
                    PaymentId = walletPaymentId,
                    UserId = UserId,
                    OrderId = openOrder.OrderId,
                    Amount = openOrder.TotalCartPrice()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var walletDeadline = DateTime.UtcNow.AddSeconds(3);
            while (DateTime.UtcNow < walletDeadline)
            {
                await Task.Delay(300, HttpContext.RequestAborted);

                var walletStatus = await Bus.InvokeAsync<WalletPaymentStatusDto>(
                    new GetWalletPaymentStatusQuery { PaymentId = walletPaymentId, UserId = UserId });

                if (walletStatus.Status == "Succeeded")
                    return Ok(new PayOrderResponseDto { WalletPayment = true, Success = true, PaymentId = walletPaymentId });

                if (walletStatus.Status == "Failed")
                    return BadRequest(new { message = walletStatus.Message });
            }

            return Ok(new PayOrderResponseDto { WalletPayment = true, Pending = true, PaymentId = walletPaymentId });
        }

        // ---- پرداخت آنلاین ----
        var paymentId = Guid.NewGuid();

        await Bus.InvokeAsync(new StartOnlinePaymentCommand
        {
            PaymentId = paymentId,
            UserId = UserId,
            OrderId = openOrder.OrderId,
            Amount = openOrder.TotalCartPrice()
        });

        return Ok(new PayOrderResponseDto
        {
            WalletPayment = false,
            Authority = paymentId.ToString("N"),
            PaymentId = paymentId,
            Order = openOrder
        });
    }

    [HttpGet("wallet-status")]
    public async Task<ActionResult<WalletPaymentStatusDto>> WalletStatus(Guid paymentId)
    {
        var status = await Bus.InvokeAsync<WalletPaymentStatusDto>(
            new GetWalletPaymentStatusQuery { PaymentId = paymentId, UserId = UserId });
        return Ok(status);
    }

    [HttpPost("pay-order-callback")]
    public async Task<ActionResult<PayOrderResponseDto>> PayOrderCallback(string authority, bool isSuccess)
    {
        if (!Guid.TryParse(authority, out var paymentId))
            return BadRequest(new { message = "تراکنش نامعتبر است." });

        await Bus.InvokeAsync(new MarkOnlinePaymentResultCommand(
            paymentId, isSuccess, isSuccess ? null : "کاربر پرداخت را در درگاه بانک لغو کرد."));

        if (!isSuccess)
            return BadRequest(new { message = "پرداخت شما ناموفق بود." });

        var deadline = DateTime.UtcNow.AddSeconds(3);
        while (DateTime.UtcNow < deadline)
        {
            await Task.Delay(300, HttpContext.RequestAborted);

            var status = await Bus.InvokeAsync<WalletPaymentStatusDto>(
                new GetWalletPaymentStatusQuery { PaymentId = paymentId, UserId = UserId });

            if (status.Status == "Succeeded")
                return Ok(new PayOrderResponseDto { WalletPayment = false, Success = true, PaymentId = paymentId });

            if (status.Status == "Failed")
                return BadRequest(new { message = status.Message });
        }

        return Ok(new PayOrderResponseDto { WalletPayment = false, Pending = true, PaymentId = paymentId });
    }

    [HttpPost("add-balance")]
    public async Task<ActionResult<AddBalanceResponseDto>> AddBalance(DepositBalanceDto dto)
    {
        if (dto.Amount <= 0)
            return BadRequest(new { message = "مبلغ باید بزرگ‌تر از صفر باشد." });

        var isProfileComplete = await Bus.InvokeAsync<bool>(new IsProfileDetailCompletedQuery { UserId = UserId });
        if (!isProfileComplete)
            return Ok(new AddBalanceResponseDto { NeedsProfile = true });

        var authority = Guid.NewGuid().ToString("N");
        await Bus.InvokeAsync(new CreatePendingTransactionCommand
        {
            UserId = UserId,
            OrderId = null,
            Amount = dto.Amount,
            Authority = authority
        });

        return Ok(new AddBalanceResponseDto { NeedsProfile = false, Authority = authority });
    }

    [HttpPost("pay-balance-callback")]
    public async Task<ActionResult<bool>> PayBalanceCallback(string authority, bool isSuccess)
    {
        var pendingTransaction = await Bus.InvokeAsync<Domain.Entities.Order.PaymentTransaction?>(new GetTransactionByAuthorityQuery { Authority = authority });
        if (pendingTransaction == null)
            return BadRequest(new { message = "تراکنش نامعتبر است." });

        if (!isSuccess)
        {
            await Bus.InvokeAsync(new MarkTransactionAsFailedCommand { TransactionId = pendingTransaction.Id });
            return BadRequest(new { message = "پرداخت شما ناموفق بود." });
        }

        var isDeposited = await Bus.InvokeAsync<bool>(new DepositWalletBalanceCommand { PaymentTransaction = pendingTransaction });

        return isDeposited
            ? Ok(true)
            : BadRequest(new { message = "خطا در شارژ کیف پول." });
    }
}