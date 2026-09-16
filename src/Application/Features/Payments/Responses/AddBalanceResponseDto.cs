// Application/Features/Payments/Responses/AddBalanceResponseDto.cs
namespace Application.Features.Payments.Responses;

public class AddBalanceResponseDto
{
    public bool NeedsProfile { get; set; }
    public string? Authority { get; set; }
}