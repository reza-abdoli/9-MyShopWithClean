// // KafkaContracts/Payments/Events.cs
// namespace KafkaContracts.Payments;

// public record PaymentRegisteredEvent(Guid PaymentId);
// public record PaymentRegistrationFailedEvent(Guid PaymentId, string Reason);
// public record PaymentVoidedEvent(Guid PaymentId);
// public record PaymentVoidFailedEvent(Guid PaymentId, string Reason); // ← جدید

namespace KafkaContracts.Payments;

public record PaymentRegisteredEvent(Guid Id);
public record PaymentRegistrationFailedEvent(Guid Id, string Reason);
public record PaymentVoidedEvent(Guid Id);
public record PaymentVoidFailedEvent(Guid Id, string Reason);


public record OnlinePaymentConfirmedEvent(Guid Id);
public record OnlinePaymentDeclinedEvent(Guid Id, string Reason);