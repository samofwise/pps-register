using Aspire.Hosting;
using Aspire.Hosting.AWS;

var builder = DistributedApplication.CreateBuilder(args);

var awsRegion = "ap-southeast-2";
var queueUrl = "https://sqs.ap-southeast-2.amazonaws.com/904581404707/PPSUploaderQueue";


var uploader = builder.AddProject<Projects.PPSRegister_PPSUploader>("ppsuploader")
    .WithEnvironment("AWS__Region", awsRegion)
    .WithEnvironment("AWS__SQS__QueueUrl", queueUrl);

var api = builder.AddProject<Projects.PPSRegister_Api>("api")
    .WithReference(uploader)
    .WithEnvironment("AWS__Region", awsRegion)
    .WithEnvironment("AWS__SQS__QueueUrl", queueUrl)
    .WithHttpsEndpoint(port: 5000);

// builder.AddNpmApp("reactvite", "../AspireJavaScript.Vite")
//   .WithReference(api)
//   .WithEnvironment("BROWSER", "none")
//   .WithHttpEndpoint(env: "VITE_PORT")
//   .WithExternalHttpEndpoints()
//   .PublishAsDockerFile();

builder.Build().Run();
