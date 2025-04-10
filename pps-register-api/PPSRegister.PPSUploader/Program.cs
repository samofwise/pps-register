using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using PPSRegister.Data;
using PPSRegister.PPSUploader.Services;
using PPSRegister.PPSUploader.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Move these to environment variables
builder.Services.AddAWSService<IAmazonSQS>(new AWSOptions
{
  Region = RegionEndpoint.APSoutheast2,
  Profile = "development-user"
});

builder.Services.AddDbContextFactory<PPSRegisterDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("PPSRegister"));
});

builder.Services.AddSingleton<IPPSUploadProcessingService, PPSUploadProcessingService>();
builder.Services.AddHostedService<PPSUploadWorker>();

var host = builder.Build();
host.Run();
