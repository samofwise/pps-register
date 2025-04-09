using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace PPSRegister.Tests;

public class SqlServerFixture : IAsyncLifetime
{
  private const string Password = "Str0ngP@ssw0rd!2024#Test";
  public MsSqlContainer Container { get; }

  public SqlServerFixture()
  {
    Container = new MsSqlBuilder()
        .WithPassword(Password)
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithCleanUp(true)
        .Build();
  }

  public string GetConnectionString()
  {
    var builder = new SqlConnectionStringBuilder(Container.GetConnectionString())
    {
      TrustServerCertificate = true
    };
    return builder.ConnectionString;
  }

  public async Task InitializeAsync()
  {
    await Container.StartAsync();
  }

  public async Task DisposeAsync()
  {
    if (Container != null)
    {
      await Container.DisposeAsync();
    }
  }
}
