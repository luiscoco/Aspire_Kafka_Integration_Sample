# .NET Aspire 9.1. integration with Kafka

**Apache Kafka** is an open-source distributed event streaming platform.

The .NET Aspire Apache Kafka integration enables you to connect to existing Kafka instances, or create new instances from .NET

For more information about this post visit the official web site:

https://learn.microsoft.com/en-us/dotnet/aspire/messaging/kafka-integration?tabs=dotnet-cli

https://github.com/dotnet/aspire

## 1. Prerrequisites

### 1.1. Install .NET 9

Visit this web site (https://dotnet.microsoft.com/es-es/download/dotnet/9.0) and download **Windows x64 SDK 9.0.202**

![image](https://github.com/user-attachments/assets/87e72641-7c88-4839-9bdb-91f64568c20a)

### 1.2. Install Visual Studio 2022 v17.3 Community Edition

https://visualstudio.microsoft.com/downloads/

![image](https://github.com/user-attachments/assets/653307c3-fe36-43c0-ac29-505d4dead3dd)

### 1.3. Install and run Docker Desktop for Windows

https://docs.docker.com/desktop/setup/install/windows-install/

![image](https://github.com/user-attachments/assets/378793f9-3705-4ca0-8761-0f546f258031)

## 2. Create an Aspire Empty application

![image](https://github.com/user-attachments/assets/df18c938-db3c-4d88-809e-8fb4c4e4662a)

![image](https://github.com/user-attachments/assets/be370fd2-373e-4770-9a51-dc7e297a7f1f)

We can see now the solution folders structure

![image](https://github.com/user-attachments/assets/a4f32b71-9be2-48e0-b259-d68f4ba2efbe)

Add Nuget Package **Aspire.Hosting.Kafka** in the **AppHost** project

![image](https://github.com/user-attachments/assets/cac3ef21-e576-4317-a58b-5edb344cdeb8)

**Aspire.Hosting.Kafka** library provides extension methods and resource definitions for a .NET Aspire AppHost to configure a **Kafka** resource.

The Nuget Package **Aspire.Hosting.Kafka** (loaded in the **AppHost** project) creates **Kafka** and **KafkaUI** images according to this data

![image](https://github.com/user-attachments/assets/029cbed5-94ef-4d6c-8dea-a0e871e6de36)

For more detailed explanation about the Nuget package **Aspire.Hosting.Kafka** visit the Aspire official github repo

https://github.com/dotnet/aspire/tree/main/src/Aspire.Hosting.Kafka

![image](https://github.com/user-attachments/assets/a8e1f270-0427-4deb-8ae5-d88df1ae4162)

## 3. Add a Console C# Application (Producer)

![image](https://github.com/user-attachments/assets/ca3c7022-a891-4347-904f-407da0e7d682)

Load Nuget Library **Aspire.Confluent.Kafka**

![image](https://github.com/user-attachments/assets/848698ec-22ba-4239-b331-b765b962ef92)

Add **.NET Aspire Orchestrator** Support

Add **ServiceDefaults** project reference

**Important note**:

For a detailed information about **Aspire.Confluent.Kafka** visit the official github repo

https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Confluent.Kafka

![image](https://github.com/user-attachments/assets/6fa9f559-c9ce-4657-9336-c3ebdd62e83b)

## 4. Add a Console C# Application (Consumer)

![image](https://github.com/user-attachments/assets/ce52bf21-286d-4a18-b48b-db9453c4d6ea)

Load Nuget Library **Aspire.Confluent.Kafka**

![image](https://github.com/user-attachments/assets/0fdf2234-3701-46a8-882a-d276220beff4)

Add **.NET Aspire Orchestrator Support**

Add **ServiceDefaults** project reference

**Important note**:

For a detailed information about **Aspire.Confluent.Kafka** visit the official github repo

https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Confluent.Kafka

## 5. Configure AppHost project middleware(Program.cs)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
    .WithKafkaUI(kafkaUi => kafkaUi.WithHostPort(8080));

builder.AddProject<Projects.Producer>("producer")
    .WithReference(kafka).WaitFor(kafka)
    .WithArgs(kafka.Resource.Name);

builder.AddProject<Projects.Consumer>("consumer")
    .WithReference(kafka).WaitFor(kafka)
    .WithArgs(kafka.Resource.Name);

builder.Build().Run();
```

## 6. Input "Producer" project source code 

![image](https://github.com/user-attachments/assets/22392ab2-7bd2-4594-9fff-ae7d6c5d604b)

**appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information",
      "Azure": "Warning"
    }
  },
  "Aspire": {
    "Confluent": {
      "Kafka": {
        "Producer": {
          "Config": {
            "Acks": "All"
          }
        }
      }
    }
  }
}
```

**Important note**: for more information about the configuration options, see the **ConfigurationSchemal.json** file content 

This file is inside the offical Aspire github repo in the folder **Components** and project **Aspire.Confluent.Kafka**

![image](https://github.com/user-attachments/assets/c49173c3-0588-4b2f-baeb-10414fb1ba80)

**Program.cs(Producer middleware)**

```csharp
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddKafkaProducer<string, string>("kafka");
builder.AddKafkaProducer<Null, string>("kafka");

builder.Services.AddHostedService<IntermittentProducerWorker>();
builder.Services.AddHostedService<ContinuousProducerWorker>();

builder.Build().Run();
```

**ContinuousProducerWorker.cs**

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Producer;

internal sealed class ContinuousProducerWorker(IProducer<Null, string> producer, ILogger<ContinuousProducerWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        long i = 0;
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var message = new Message<Null, string> { Value = $"Hello, World! {i}" };
            producer.Produce("demo-topic", message);
            logger.LogInformation($"{producer.Name} sent message '{message.Value}'");
            i++;
        }
    }
}
```

**IntermittentProducerWorker.cs**

```chsarp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Producer;

internal sealed class IntermittentProducerWorker(IProducer<string, string> producer, ILogger<IntermittentProducerWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long i = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            for (int j = 0; j < 1000; j++, i++)
            {
                var message = new Message<string, string> { Value = $"Hello, World! {i}" };
                producer.Produce("demo-topic", message);
            }

            producer.Flush(stoppingToken);

            logger.LogInformation($"{producer.Name} sent 1000 messages, waiting 10 s");

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
```


## 7. Input "Consumer" project source code

![image](https://github.com/user-attachments/assets/7cb2d991-9b63-4195-967d-882988acf304)

**appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information",
      "Azure": "Warning"
    }
  },
  "Aspire": {
    "Confluent": {
      "Kafka": {
        "Consumer": {
          "Config": {
            "AutoOffsetReset": "Earliest",
            "GroupId": "aspire"
          }
        }
      }
    }
  }
}
```

**program.cs**

```csharp
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
```

**ConsumerWorker.cs**

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer;

internal sealed class ConsumerWorker(IConsumer<Ignore, string> consumer, ILogger<ConsumerWorker> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long i = 0;
        return Task.Factory.StartNew(async () =>
        {
            consumer.Subscribe("demo-topic");
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Ignore, string>? result = default;
                try
                {
                    result = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (result is not null)
                    {
                        logger.LogInformation($"Consumed message [{result.Message?.Key}] = {result.Message?.Value}");
                    }
                }
                catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    await Task.Delay(100);
                    continue;
                }

                i++;
                if (i % 1000 == 0)
                {
                    logger.LogInformation($"Received {i} messages. current offset is '{result!.Offset}'");
                }
            }
        }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }
}
```
## 8. Run the Application and verify the results

Before running the application we have to run **Docker Desktop**

![image](https://github.com/user-attachments/assets/b875eaed-09f7-4ba9-9582-3a13744f746c)

We set **AppHost** project as the **StartUp project**

![image](https://github.com/user-attachments/assets/f5c5cb44-0e88-4f21-b069-45241632efd3)

We run the application and automatically opens the **Aspire Dashboard**

We have to wait for some time until the Kafka and KafkaUI docker images are download and after the docker containers are run

![image](https://github.com/user-attachments/assets/e1901f73-ad55-48ae-8638-7d2698931a44)

We check in **Docker Desktop** the **docker images** were successfully downloaded

![image](https://github.com/user-attachments/assets/505f9352-4af2-48ff-93b8-822ceded6192)

also you can execute this command to see the **docker images**

```
docker images
```

![image](https://github.com/user-attachments/assets/6dc78cae-d7ae-4c4a-9ccf-3c1511e55d8c)

We can also verify in **Docker Desktop** the **containers** are running

![image](https://github.com/user-attachments/assets/4710a141-d484-4dcf-9206-5c57dfe65ca9)

also you can execute this command to see the **running docker containers**

```
docker ps
```

![image](https://github.com/user-attachments/assets/dc23b2a9-e010-4258-9f88-8ebaa5324b95)

After some time we confirm all the applications and container are running properly

![image](https://github.com/user-attachments/assets/88f84577-55c2-4d23-878f-550f533873db)

Now we click on the **Console** icon in the left hand side menu

We can select the **Consumer** application console output

![image](https://github.com/user-attachments/assets/582b6a5f-562d-4411-a7cf-52a9bbb088f6)

Or we can select the **Producer** application console output

![image](https://github.com/user-attachments/assets/1459aeba-7028-47ed-9eff-46e2f9346372)

We also can navigate to the **kafkaUI**

![image](https://github.com/user-attachments/assets/0d8ae654-5f34-4900-9e27-db2bdecea641)

We can verify the kafka information

![image](https://github.com/user-attachments/assets/f87d8dd8-2736-437e-abad-f1117a9b459e)

We can visit the **kafka Brokers** information and configuration

![image](https://github.com/user-attachments/assets/7cb4019e-e7c6-4f85-8479-6bca5e722120)

![image](https://github.com/user-attachments/assets/252090d4-e864-420e-9e90-25cc877b96a3)

We verify the **kafka topics**

![image](https://github.com/user-attachments/assets/1780f899-c561-4355-9be1-3151b586826c)

We inspect the **messages sent and received** in the topic

![image](https://github.com/user-attachments/assets/e0b1c3c8-cee5-4b8b-9dff-c2534b031d2d)

We can **Produce** a new message clicking on the **Produce Message** button

We also see the **kafka consumers**

![image](https://github.com/user-attachments/assets/5191c836-6b1a-41c6-91d2-b79d408b11e0)

