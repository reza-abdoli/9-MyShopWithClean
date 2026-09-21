namespace Application.Features.Payments;

public interface IOnlinePaymentRefunder
{
    Task<bool> RefundToWalletAsync(long paymentTransactionId);
}