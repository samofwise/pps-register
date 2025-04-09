using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using PPSRegister.Api.Services;
using PPSRegister.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PPSRegister"));
    options.UseAsyncSeeding(DatabaseSeeder.Seed);
});

builder.Services.AddAWSService<IAmazonSQS>(new AWSOptions
{
    Profile = "development-user",
    Region = RegionEndpoint.APSoutheast2
});

builder.Services.AddScoped<IPersonalPropertySecurityService, PersonalPropertySecurityService>();
builder.Services.AddScoped<IPersonalPropertySecurityUploadService, PersonalPropertySecurityUploadService>();


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

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());


using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<PPSRegisterDbContext>();
await context.Database.EnsureCreatedAsync();

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();