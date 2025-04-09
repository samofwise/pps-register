using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using PPSRegister.Api.Services;
using PPSRegister.Data;
using PPSRegister.PPSUploader.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PPSRegister API",
        Version = "v1",
        Description = "Handles PPS Upload logic."
    });

});

builder.Services.AddDbContext<PPSRegisterDbContext>(options =>
    options.UseInMemoryDatabase("VehicleRegistrationDb")
    .UseAsyncSeeding(DatabaseSeeder.Seed)
    );

// Add AWS SQS services
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection("QueueSettings"));

builder.Services.AddScoped<IPersonalPropertySecurityUploadService, PersonalPropertySecurityUploadService>();

// Register HTTP client for calling PPSUploader (set by Aspire via env var)
builder.Services.AddHttpClient("PpsUploader", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["service:PpsUploader:url"]; // Aspire injects this!
    client.BaseAddress = new Uri(baseUrl!);
});

var app = builder.Build();

// Swagger and development stuff
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PPSRegister API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();