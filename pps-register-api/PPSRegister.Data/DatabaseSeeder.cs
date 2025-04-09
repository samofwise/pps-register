using Microsoft.EntityFrameworkCore;
using PPSRegister.Data.Models;

namespace PPSRegister.Data;

public static class DatabaseSeeder
{
  public static async Task<bool> Seed(DbContext context, bool force = false, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!context.Set<Client>().Any())
      {
        var clients = new List<Client>
        {
            new() { Name = "ABC Motors" },
            new() { Name = "XYZ Auto" },
            new() { Name = "123 Car Dealership" }
        };

        context.Set<Client>().AddRange(clients);
        await context.SaveChangesAsync(cancellationToken);
      }

      return true;
    }
    catch
    {
      return false;
    }
  }
}