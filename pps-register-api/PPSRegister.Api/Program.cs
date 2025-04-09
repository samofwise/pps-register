using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
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

// Move these to environment variables
builder.Services.AddAWSService<IAmazonSQS>(new AWSOptions
{
    Region = RegionEndpoint.APSoutheast2,
    Credentials = new SessionAWSCredentials(
        "ASIA5FHKA3ARWXBQFYTK",
        "fcOPRrxagvtQOylGfWTycTQmfTvPCQ19H/VU5Pcf",
        "IQoJb3JpZ2luX2VjEB8aDmFwLXNvdXRoZWFzdC0yIkgwRgIhAM1tgqSaDSi4tX9PV+MBKccwWMsTrJ7uwM3d9B4k3Rs7AiEA9vH2oGESctN+f0Mqs9dyV/f+Wxt7WzZ/zg0zCmIWrmkqhgMImP//////////ARADGgw5MDQ1ODE0MDQ3MDciDJ0OBBts5C3u2XiaFiraAuIAAtzRBU494/JKHwYQhwRVOzk9ZI9tpBFjBxiZsO5oHPobNku7dyqixuqOJ697tcueAqERtOtyl8kdyksl5Itb10Bws1uJkULRQSwAB8B+mjounRRkMmWC0hlUssOnscqIh5SS6v2bzHZHMWJupNm8aO7k3lLNzPrvoh92OyFIFCpLU1ashxYuiiM6QTLb+DK/4mdhyQ6a6j7IGrJ/Ta/sW/qMn7rptFcYSb5iZVjqfufDN9bK7FJJGdVNH1qPDvE0v6aZe8YnNb75S+0xFfJR0WQ1WhH/Rf2JO7pDdxBczNmejk7NJx8e1yXyn1Jw8dpiDsaTbr7YsWfzgrdmZl6zLalEuNJw8yQB0Q2gpkrulSdWrjTtRcMi97Uhb4As4s9oRHGhxpVgXLwLbEA7X12fwGuYLecWih59Hea9wxhXHZrXnaAtVhJpsfDc8kI5ffjmdOmzFlX1UKEw9P/bvwY6pgElSRCr5/AsY+/z3JSeI2AAblf5JPPAaKLAm/9MZU/BcBsv8eJDgmtBX81NKWLskCakEq+dSNhNzN2eXIpVt2NRhV7Dd60RfAuzibXZQleLJERFH+C64fRnQPRlL0v3oyJsf3ROFEPJdmD6MY+naMlgJX1ZnRaboczMCFSUWSq38QgXZ5HSYAyGrmKkinfOouUwL4FADBrodLqiYLyEeri3vkrBYDfE"
    )
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