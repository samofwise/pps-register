using Aspire.Hosting;
using Aspire.Hosting.AWS;

var builder = DistributedApplication.CreateBuilder(args);

var awsRegion = "ap-southeast-2";
var queueUrl = "https://sqs.ap-southeast-2.amazonaws.com/904581404707/PPSUploaderQueue";


var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("PPSRegister");


var uploader = builder.AddProject<Projects.PPSRegister_PPSUploader>("ppsuploader")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("AWS__Region", awsRegion)
    .WithEnvironment("AWS__SQS__QueueUrl", queueUrl);

var api = builder.AddProject<Projects.PPSRegister_Api>("api")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(uploader)
    .WithEnvironment("AWS__Region", awsRegion)
    .WithEnvironment("AWS__SQS__QueueUrl", queueUrl);

// builder.AddNpmApp("reactvite", "../AspireJavaScript.Vite")
//   .WithReference(api)
//   .WithEnvironment("BROWSER", "none")
//   .WithHttpEndpoint(env: "VITE_PORT")
//   .WithExternalHttpEndpoints()
//   .PublishAsDockerFile();

builder.Build().Run();
