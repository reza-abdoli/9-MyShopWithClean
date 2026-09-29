// Application/Features/Payments/Responses/PayOrderResponseDto.cs
using Application.DTOs.Order;

namespace Application.Features.Payments.Responses;

public class PayOrderResponseDto
{
    public bool Pending { get; set; }
    public Guid? PaymentId { get; set; }
    public bool WalletPayment { get; set; }
    public bool Success { get; set; }
    public string? Authority { get; set; }
    public OpenOrderDto? Order { get; set; }
}