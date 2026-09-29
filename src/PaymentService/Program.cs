// PaymentService/Program.cs
using JasperFx;
using KafkaContracts;
using KafkaContracts.Payments;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Kafka;
using Wolverine.SqlServer;

var builder = Host.CreateApplicationBuilder(args);

var connStr = builder.Configuration.GetConnectionString("PaymentConnection")
    ?? throw new InvalidOperationException("Connection string 'PaymentConnection' not found.");
var kafkaAddress = builder.Configuration.GetValue<string>("KafkaAddress")!;

builder.Services.AddDbContextWithWolverineIntegration<PaymentDbContext>(o => o.UseSqlServer(connStr));

builder.UseWolverine(opts =>
{
    opts.AutoBuildMessageStorageOnStartup = AutoCreate.None; // فقط برای تست
    opts.PersistMessagesWithSqlServer(connStr, "wolverine");
    opts.UseEntityFrameworkCoreTransactions();

    opts.UseKafka(kafkaAddress).AutoProvision();

    opts.ListenToKafkaTopic(Topics.PaymentCommands).ProcessInline();

    opts.PublishMessage<PaymentRegisteredEvent>().ToKafkaTopic(Topics.PaymentEvents).UseDurableOutbox();
    opts.PublishMessage<PaymentRegistrationFailedEvent>().ToKafkaTopic(Topics.PaymentEvents).UseDurableOutbox();
    opts.PublishMessage<PaymentVoidedEvent>().ToKafkaTopic(Topics.PaymentEvents).UseDurableOutbox();

    opts.PublishMessage<OnlinePaymentConfirmedEvent>().ToKafkaTopic(Topics.PaymentEvents).UseDurableOutbox();   // ← جدید
    opts.PublishMessage<OnlinePaymentDeclinedEvent>().ToKafkaTopic(Topics.PaymentEvents).UseDurableOutbox();     // ← جدید
});

await builder.Build().RunAsync();