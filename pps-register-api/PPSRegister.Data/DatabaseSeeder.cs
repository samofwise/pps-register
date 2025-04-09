using Microsoft.EntityFrameworkCore;
using PPSRegister.Data.Models;

namespace PPSRegister.Data;

public static class DatabaseSeeder
{
  public static async Task<bool> Seed(DbContext context, bool force = false, CancellationToken cancellationToken = default)
  {
    var strategy = context.Database.CreateExecutionStrategy();

    await strategy.ExecuteAsync(async () =>
    {
      using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
      try
      {
        if (!context.Set<Client>().Any())
        {
          var clients = new List<Client>
          {
              new() { Id = 1, Name = "ABC Motors" },
              new() { Id = 2, Name = "XYZ Auto" },
              new() { Id = 3, Name = "123 Car Dealership" }
          };

          context.Set<Client>().AddRange(clients);
        }

        if (!context.Set<PersonalPropertySecurityUpload>().Any())
        {
          var uploads = new List<PersonalPropertySecurityUpload>
          {
            new()
            {
              FileName = "registrations_2024_01.csv",
              Submitted = 150,
              Invalid = 5,
              Processed = 145,
              Updated = 30,
              Added = 115,
              ClientId = 1
            },
            new()
            {
              FileName = "registrations_2024_02.csv",
              ClientId = 1
            },
            new()
            {
              FileName = "registrations_2024_02.csv",
              Submitted = 200,
              Invalid = 8,
              Processed = 192,
              Updated = 45,
              Added = 147,
              ClientId = 1
            },
            new()
            {
              FileName = "registrations_2024_05.csv",
              Submitted = 190,
              Invalid = 4,
              Processed = 186,
              Updated = 35,
              Added = 151,
              ClientId = 3
            },
            new()
            {
              FileName = "registrations_2024_06.csv",
              Submitted = 210,
              Invalid = 7,
              Processed = 203,
              Updated = 40,
              Added = 163,
              ClientId = 3
            },
            new()
            {
              FileName = "registrations_2024_06.csv",
              ClientId = 3
            }
          };
          context.Set<PersonalPropertySecurityUpload>().AddRange(uploads);
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
      }
      catch
      {
        await transaction.RollbackAsync(cancellationToken);
        throw;
      }
    });

    return true;
  }
}