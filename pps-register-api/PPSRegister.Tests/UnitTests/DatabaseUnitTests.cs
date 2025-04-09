using PPSRegister.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PPSRegister.Data.Models;

namespace PPSRegister.Tests.UnitTests;

public class DatabaseUnitTests : IClassFixture<SqlServerFixture>
{
  private readonly DbContextOptions<PPSRegisterDbContext> _options;

  public DatabaseUnitTests(SqlServerFixture fixture)
  {
    _options = new DbContextOptionsBuilder<PPSRegisterDbContext>()
        .UseSqlServer(fixture.GetConnectionString())
        .Options;

    using var context = new PPSRegisterDbContext(_options);
    context.Database.EnsureCreated();
  }


  [Fact]
  public async Task Database_ShouldPreventDuplicateFileNames()
  {
    using var context = new PPSRegisterDbContext(_options);

    var firstUpload = new PersonalPropertySecurityUpload
    {
      FileName = "test.csv",
      ClientId = 1
    };

    var duplicateUpload = new PersonalPropertySecurityUpload
    {
      FileName = "test.csv",
      ClientId = 1
    };

    await context.PersonalPropertySecurityUploads.AddAsync(firstUpload);
    await context.SaveChangesAsync();

    await context.PersonalPropertySecurityUploads.AddAsync(duplicateUpload);

    // Assert
    await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

    var uploads = await context.PersonalPropertySecurityUploads.ToListAsync();
    Assert.Single(uploads);
    Assert.Equal("test.csv", uploads[0].FileName);
  }


  [Fact]
  public async Task Database_ShouldPreventDuplicateVIN()
  {
    using var context = new PPSRegisterDbContext(_options);

    var registration1 = new PersonalPropertySecurity
    {
      GrantorFirstName = "John",
      GrantorMiddleNames = "Middle",
      GrantorLastName = "Doe",
      VIN = "1HGCM82633A123456",
      RegistrationStartDate = DateTime.Today,
      RegistrationDuration = "7",
      SpgAcn = "123456789",
      SpgOrganizationName = "Test Org"
    };

    var registration2 = new PersonalPropertySecurity
    {
      GrantorFirstName = "Jane",
      GrantorMiddleNames = "Middle",
      GrantorLastName = "Smith",
      VIN = "1HGCM82633A123456",
      RegistrationStartDate = DateTime.Today,
      RegistrationDuration = "7",
      SpgAcn = "987654321",
      SpgOrganizationName = "Test Org 2"
    };

    await context.PersonalPropertySecurities.AddAsync(registration1);
    await context.SaveChangesAsync();

    await context.PersonalPropertySecurities.AddAsync(registration2);
    await Assert.ThrowsAsync<DbUpdateException>(async () => await context.SaveChangesAsync());
  }

  [Fact]
  public async Task Database_ShouldPreventDuplicateGrantorVINSPG()
  {
    using var context = new PPSRegisterDbContext(_options);

    var registration1 = new PersonalPropertySecurity
    {
      GrantorFirstName = "John",
      GrantorMiddleNames = "Middle",
      GrantorLastName = "Doe",
      VIN = "1HGCM82633A123456",
      RegistrationStartDate = DateTime.Today,
      RegistrationDuration = "7",
      SpgAcn = "123456789",
      SpgOrganizationName = "Test Org"
    };

    var registration2 = new PersonalPropertySecurity
    {
      GrantorFirstName = "John",
      GrantorMiddleNames = "Middle",
      GrantorLastName = "Doe",
      VIN = "1HGCM82633A123456",
      RegistrationStartDate = DateTime.Today,
      RegistrationDuration = "7",
      SpgAcn = "123456789",
      SpgOrganizationName = "Test Org"
    };

    await context.PersonalPropertySecurities.AddAsync(registration1);
    await context.SaveChangesAsync();

    await context.PersonalPropertySecurities.AddAsync(registration2);
    await Assert.ThrowsAsync<DbUpdateException>(async () => await context.SaveChangesAsync());
  }


  [Fact]
  public async Task Database_ShouldEnforceFieldLengths()
  {
    using var context = new PPSRegisterDbContext(_options);

    var registration = new PersonalPropertySecurity
    {
      GrantorFirstName = new string('a', 36), // Exceeds 35 chars
      GrantorMiddleNames = new string('b', 76), // Exceeds 75 chars
      GrantorLastName = new string('c', 36), // Exceeds 35 chars
      VIN = new string('d', 18), // Exceeds 17 chars
      RegistrationStartDate = DateTime.Today,
      RegistrationDuration = "7",
      SpgAcn = new string('e', 10),
      SpgOrganizationName = new string('f', 76) // Exceeds 75 chars
    };

    await context.PersonalPropertySecurities.AddAsync(registration);
    await Assert.ThrowsAsync<DbUpdateException>(async () => await context.SaveChangesAsync());
  }
}

