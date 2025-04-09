using Amazon.SQS;
using PPSRegister.PPSUploader.Services;
using PPSRegister.PPSUploader.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddScoped<IPPSUploadProcessingService, PPSUploadProcessingService>();

builder.Services.AddHostedService<PPSUploadWorker>();


var host = builder.Build();
host.Run();
