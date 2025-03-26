// See https://// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Confluent.Kafka;
using Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddKafkaConsumer<Ignore, string>("kafka", settings =>
{
    settings.Config.GroupId = "aspire";
    settings.Config.AutoOffsetReset = AutoOffsetReset.Earliest;
});

builder.Services.AddHostedService<ConsumerWorker>();

builder.Build().Run();
