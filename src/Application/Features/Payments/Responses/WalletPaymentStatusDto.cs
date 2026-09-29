// Application/Features/Payments/Responses/WalletPaymentStatusDto.cs
namespace Application.Features.Payments.Responses;

public class WalletPaymentStatusDto
{
    public string Status { get; set; } = "Processing";   // Processing | Succeeded | Failed
    public string? Message { get; set; }
}