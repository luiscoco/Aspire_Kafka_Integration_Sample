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

Add Nuget Package **Aspire.Hosting.Kafka** in the **AppHost** project

**Aspire.Hosting.Kafka** library provides extension methods and resource definitions for a .NET Aspire AppHost to configure a **Kafka** resource.

The Nuget Package **Aspire.Hosting.Kafka** (loaded in the **AppHost** project) creates **Kafka** and **KafkaUI** images according to this data

![image](https://github.com/user-attachments/assets/029cbed5-94ef-4d6c-8dea-a0e871e6de36)

For more detailed explanation about the Nuget package **Aspire.Hosting.Kafka** visit the Aspire official github repo

https://github.com/dotnet/aspire/tree/main/src/Aspire.Hosting.Kafka

![image](https://github.com/user-attachments/assets/a8e1f270-0427-4deb-8ae5-d88df1ae4162)

## 3. Add a Console C# Application (Producer)

Load Nuget Library **Aspire.Confluent.Kafka**

For a detailed information about **Aspire.Confluent.Kafka** visit the official github repo

https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Confluent.Kafka

![image](https://github.com/user-attachments/assets/6fa9f559-c9ce-4657-9336-c3ebdd62e83b)

Add **.NET Aspire Orchestrator** Support

Add **ServiceDefaults** project reference


## 4. Add a Console C# Application (Producer)

Load Nuget Library **Aspire.Confluent.Kafka**

For a detailed information about **Aspire.Confluent.Kafka** visit the official github repo

https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Confluent.Kafka

Add **.NET Aspire Orchestrator Support**

Add **ServiceDefaults** project reference

## 5. Configure AppHost project middleware(Program.cs)


## 6. Input Producer project source code 


## 7. Input Consumer project source code


## 8. Run the Application and verify the results





