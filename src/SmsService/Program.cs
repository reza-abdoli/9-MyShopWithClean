// src/SmsService/Program.cs
using Confluent.Kafka;
using KafkaContracts;
using SmsService;
using Wolverine;
using Wolverine.Kafka;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ISmsSender, FileSmsSender>();

builder.UseWolverine(opts =>
{
    opts.UseKafka("localhost:9092").AutoProvision();

    opts.ListenToKafkaTopic(Topics.Sms)
        .ProcessInline() // offset بعد از تموم شدن Handler ثبت می‌شه
        .ConfigureConsumer(c => c.GroupId = "sms-service");
});

await builder.Build().RunAsync();