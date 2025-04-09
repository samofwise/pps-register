using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using PPSRegister.Data;
using PPSRegister.PPSUploader.Services;
using PPSRegister.PPSUploader.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAWSService<IAmazonSQS>(new AWSOptions
{
  Profile = "development-user",
  Region = RegionEndpoint.APSoutheast2
});

builder.Services.AddDbContextFactory<PPSRegisterDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("PPSRegister"));
});

builder.Services.AddSingleton<IPPSUploadProcessingService, PPSUploadProcessingService>();
builder.Services.AddHostedService<PPSUploadWorker>();

var host = builder.Build();
host.Run();
