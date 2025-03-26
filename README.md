# .NET Aspire 9.1. integration with Kafka

**Apache Kafka** is an open-source distributed event streaming platform.

The .NET Aspire Apache Kafka integration enables you to connect to existing Kafka instances, or create new instances from .NET

For more information about this post visit the official web site:

https://learn.microsoft.com/en-us/dotnet/aspire/messaging/kafka-integration?tabs=dotnet-cli

https://github.com/dotnet/aspire

## 1. Prerrequisites

### 1.1. Install .NET 9

### 1.2. Install Visual Studio 2022 v17.3

### 1.3. Install and run Docker Desktop for Windows

## 2. Create an Aspire Empty application

![image](https://github.com/user-attachments/assets/df18c938-db3c-4d88-809e-8fb4c4e4662a)

![image](https://github.com/user-attachments/assets/be370fd2-373e-4770-9a51-dc7e297a7f1f)

We can see now the solution folders structure

![image](https://github.com/user-attachments/assets/a4f32b71-9be2-48e0-b259-d68f4ba2efbe)

Add Nuget Package **Aspire.Hosting.Kafka** in the **AppHost** project

**Aspire.Hosting.Kafka** library provides extension methods and resource definitions for a .NET Aspire AppHost to configure a **Kafka** resource.

The Nuget Package **Aspire.Hosting.Kafka** (loaded in the **AppHost** project) creates **Kafka** and **KafkaUI** images according to this data

![image](https://github.com/user-attachments/assets/029cbed5-94ef-4d6c-8dea-a0e871e6de36)

For more detailed explanation about the Nuget package **Aspire.Hosting.Kafka** visit the Aspire official github repo

https://github.com/dotnet/aspire/tree/main/src/Aspire.Hosting.Kafka

![image](https://github.com/user-attachments/assets/a8e1f270-0427-4deb-8ae5-d88df1ae4162)

## 3. Add a Console C# Application (Producer)

![image](https://github.com/user-attachments/assets/ca3c7022-a891-4347-904f-407da0e7d682)

Load Nuget Library **Aspire.Confluent.Kafka**

For a detailed information about **Aspire.Confluent.Kafka** visit the official github repo

https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Confluent.Kafka

![image](https://github.com/user-attachments/assets/6fa9f559-c9ce-4657-9336-c3ebdd62e83b)

Add **.NET Aspire Orchestrator** Support

Add **ServiceDefaults** project reference


## 4. Add a Console C# Application (Consumer)

![image](https://github.com/user-attachments/assets/ce52bf21-286d-4a18-b48b-db9453c4d6ea)

Load Nuget Library **Aspire.Confluent.Kafka**

For a detailed information about **Aspire.Confluent.Kafka** visit the official github repo

https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Confluent.Kafka

Add **.NET Aspire Orchestrator Support**

Add **ServiceDefaults** project reference

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



## 7. Input "Consumer" project source code

```csharp

```

## 8. Run the Application and verify the results





