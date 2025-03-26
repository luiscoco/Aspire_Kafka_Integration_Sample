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
