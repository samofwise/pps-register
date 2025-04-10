using Aspire.Hosting;
using Aspire.Hosting.AWS;

var builder = DistributedApplication.CreateBuilder(args);

var awsRegion = "ap-southeast-2";
var queueUrl = "https://sqs.ap-southeast-2.amazonaws.com/904581404707/PPSUploaderQueue";

var sql = builder.AddSqlServer("sql")
  .WithLifetime(ContainerLifetime.Persistent)
  .WithContainerName("aspire-pps-register-sql");

var db = sql.AddDatabase("PPSRegister");

var uploader = builder.AddProject<Projects.PPSRegister_PPSUploader>("aspire-pps-register-uploader")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("AWS__Region", awsRegion)
    .WithEnvironment("AWS__SQS__QueueUrl", queueUrl);

var api = builder.AddProject<Projects.PPSRegister_Api>("aspire-pps-register-api")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(uploader)
    .WithHttpsEndpoint(name: "api", port: 4001)
    .WithEnvironment("AWS__Region", awsRegion)
    .WithEnvironment("AWS__SQS__QueueUrl", queueUrl);

builder.AddNpmApp("aspire-pps-register-app", "../../pps-register-app", "start")
  .WithReference(api)
  .WaitFor(api)
  .WithEnvironment("BROWSER", "none")
  .WithEnvironment("VITE_API_URL", api.GetEndpoint("api"))
  .WithHttpEndpoint(port: 3500, env: "VITE_PORT")
  .WithExternalHttpEndpoints()
  .WithEnvironment("NODE_TLS_REJECT_UNAUTHORIZED", "0");

builder.Build().Run();
