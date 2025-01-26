using DesignerApp.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> cache = builder.AddRedis("cache").WithImageTag("latest").WithClearCommand();
IResourceBuilder<MongoDBServerResource> mongo = builder.AddMongoDB("mongo").WithImageTag("latest");
IResourceBuilder<RabbitMQServerResource> rabbit = builder.AddRabbitMQ("rabbit").WithImageTag("latest");
var postgress = builder.AddPostgres("postgress").WithImageTag("latest");


IResourceBuilder<ProjectResource> apirestservice = builder.AddProject<Projects.ApiRestService>("apirestservice");
IResourceBuilder<ProjectResource> commerceservice = builder.AddProject<Projects.CommerceService>("commerceservice");
IResourceBuilder<ProjectResource> constructorservice = builder.AddProject<Projects.ConstructorService>("constructorservice");
IResourceBuilder<ProjectResource> helpdeskservice = builder.AddProject<Projects.HelpdeskService>("helpdeskservice");
IResourceBuilder<ProjectResource> identityservice = builder.AddProject<Projects.IdentityService>("identityservice");
IResourceBuilder<ProjectResource> storageservice = builder.AddProject<Projects.StorageService>("storageservice");
IResourceBuilder<ProjectResource> telegramBot = builder.AddProject<Projects.Telegram_Bot_Polling>("telegram-bot-polling");

builder.AddProject<Projects.BlankBlazorApp>("blankblazorapp")
    .WithExternalHttpEndpoints()

    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(mongo)
    .WaitFor(mongo)
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WithReference(postgress)
    .WaitFor(postgress)

    .WithReference(apirestservice)
    .WaitFor(apirestservice)
    .WithReference(commerceservice)
    .WaitFor(commerceservice)
    .WithReference(constructorservice)
    .WaitFor(constructorservice)
    .WithReference(helpdeskservice)
    .WaitFor(helpdeskservice)
    .WithReference(identityservice)
    .WaitFor(identityservice)
    .WithReference(storageservice)
    .WaitFor(storageservice)
    .WithReference(telegramBot)
    .WaitFor(telegramBot)
;

builder.Build().Run();
/*
 builder.AddProject<Projects.Store>("store")
       .WithExternalHttpEndpoints()
       .WithReference(apirestservice);
 */