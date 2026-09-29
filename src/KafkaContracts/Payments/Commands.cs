// // KafkaContracts/Payments/Commands.cs
// namespace KafkaContracts.Payments;

// public record RegisterPaymentCommand(Guid PaymentId, long OrderId, long UserId, int Amount);
// public record VoidPaymentCommand(Guid PaymentId);

namespace KafkaContracts.Payments;

public record RegisterPaymentCommand(Guid Id, long OrderId, long UserId, int Amount);
public record VoidPaymentCommand(Guid Id);


public record CreateOnlinePendingPaymentCommand(Guid Id, long OrderId, long UserId, int Amount);
public record MarkOnlinePaymentResultCommand(Guid Id, bool IsSuccess, string? FailureReason);