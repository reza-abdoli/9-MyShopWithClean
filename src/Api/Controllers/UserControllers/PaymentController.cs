// Api/Controllers/UserControllers/PaymentController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Account;
using Application.Features.Orders.Requests.Queries;
using Application.Features.Payments.Requests.Commands;
using Application.Features.Payments.Requests.Queries;
using Application.Features.Payments.Responses;
using Application.Features.Users.Requests.Queries;
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

            var isSuccess = await Bus.InvokeAsync<bool>(new PayOrderWithWalletCommand
            {
                UserId = UserId,
                TotalCartPrice = openOrder.TotalCartPrice(),
                OrderId = openOrder.Order.Id
            });

            return isSuccess
                ? Ok(new PayOrderResponseDto { WalletPayment = true, Success = true })
                : BadRequest(new { message = "خطایی در پردازش پرداخت یا موجودی انبار رخ داد." });
        }

        var authority = Guid.NewGuid().ToString("N");
        await Bus.InvokeAsync(new CreatePendingTransactionCommand
        {
            UserId = UserId,
            OrderId = openOrder.Order.Id,
            Amount = openOrder.TotalCartPrice(),
            Authority = authority
        });

        return Ok(new PayOrderResponseDto { WalletPayment = false, Authority = authority, Order = openOrder });
    }

    [HttpPost("pay-order-callback")]
    public async Task<ActionResult<bool>> PayOrderCallback(string authority, bool isSuccess)
    {
        var pendingTransaction = await Bus.InvokeAsync<Domain.Entities.Order.PaymentTransaction?>(new GetTransactionByAuthorityQuery { Authority = authority });
        if (pendingTransaction == null)
            return BadRequest(new { message = "تراکنش نامعتبر است." });

        if (!isSuccess)
        {
            await Bus.InvokeAsync(new MarkTransactionAsFailedCommand { TransactionId = pendingTransaction.Id });
            return BadRequest(new { message = "پرداخت شما ناموفق بود." });
        }

        var isFinalized = await Bus.InvokeAsync<bool>(new FinalizeOnlineOrderPaymentCommand { PaymentTransaction = pendingTransaction });

        return isFinalized
            ? Ok(true)
            : BadRequest(new { message = "خطا در نهایی‌سازی سفارش. در صورت کسر مبلغ، به کیف پول برگردانده شد." });
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