using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PPSRegister.Data;
using PPSRegister.Data.Models;
using PPSRegister.PPSUploader.Services;

namespace PPSRegister.Tests.UnitTests;

public class UploadProcessingUnitTests
{
  private readonly Mock<IDbContextFactory<PPSRegisterDbContext>> _mockDbContextFactory;
  private readonly Mock<ILogger<PPSUploadProcessingService>> _mockLogger;
  private readonly PPSUploadProcessingService _service;

  private readonly PPSRegisterDbContext _assertContext;

  private readonly DbContextOptions<PPSRegisterDbContext> _options = new DbContextOptionsBuilder<PPSRegisterDbContext>()
        .UseInMemoryDatabase(databaseName: "PPSRegisterTestDb")
        .Options;

  public UploadProcessingUnitTests()
  {

    _assertContext = new PPSRegisterDbContext(_options);
    _assertContext.Database.EnsureCreated();

    _mockDbContextFactory = new Mock<IDbContextFactory<PPSRegisterDbContext>>();
    _mockLogger = new Mock<ILogger<PPSUploadProcessingService>>();
    _service = new PPSUploadProcessingService(_mockDbContextFactory.Object, _mockLogger.Object);
  }

  [Fact]
  public async Task ProcessUpload_WithNullMessage_ShouldNotProcess()
  {
    // Act
    await _service.ProcessUpload(null);

    // Assert
    _mockDbContextFactory.Verify(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact]
  public async Task ProcessUpload_WithValidNewRecords_ShouldAddAllRecords()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,2024-05-01,7,001 234 567,Test Company
Jane,Marie,Smith,2FMDK48C87BA12345,2024-05-02,25,002 345 678,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(2, upload.Submitted);
    Assert.Equal(2, upload.Processed);
    Assert.Equal(2, upload.Added);
    Assert.Equal(0, upload.Updated);
    Assert.Equal(0, upload.Invalid);

    var records = await _assertContext.PersonalPropertySecurities.ToListAsync();
    Assert.Equal(2, records.Count);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithDuplicateRecords_ShouldUpdateExisting()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    // Add an existing record
    var existingRecord = new PersonalPropertySecurity
    {
      GrantorFirstName = "John",
      GrantorLastName = "Doe",
      VIN = "1HGCM82633A123456",
      SpgAcn = "001 234 567",
      SpgOrganizationName = "Test Company",
      RegistrationStartDate = new DateTime(2024, 1, 1),
      RegistrationDuration = "7",
      ClientId = 1
    };
    context.PersonalPropertySecurities.Add(existingRecord);
    await context.SaveChangesAsync();

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,2024-05-01,25,001 234 567,Test New Company Name"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.SingleAsync();
    Assert.Equal(1, upload.Submitted);
    Assert.Equal(1, upload.Processed);
    Assert.Equal(0, upload.Added);
    Assert.Equal(1, upload.Updated);
    Assert.Equal(0, upload.Invalid);

    var record = await _assertContext.PersonalPropertySecurities.FirstAsync();
    Assert.Equal("25", record.RegistrationDuration);
    Assert.Equal(new DateTime(2024, 5, 1), record.RegistrationStartDate);
    Assert.Equal("Test New Company Name", record.SpgOrganizationName);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidRecords_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,invalid-date,7,001 234 567,Test Company
Jane,Marie,Smith,2FMDK48C87BA12345,2024-05-02,invalid-duration,002 345 678,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(2, upload.Submitted);
    Assert.Equal(0, upload.Processed);
    Assert.Equal(0, upload.Added);
    Assert.Equal(0, upload.Updated);
    Assert.Equal(2, upload.Invalid);

    var records = await _assertContext.PersonalPropertySecurities.ToListAsync();
    Assert.Empty(records);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidGrantorFirstName_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
,John,Doe,1HGCM82633A123456,2024-05-01,7,001 234 567,Test Company
ThisIsAVeryLongFirstNameThatExceedsTheMaximumAllowedLengthOf35Characters,Doe,Smith,2FMDK48C87BA12345,2024-05-02,25,002 345 678,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(2, upload.Submitted);
    Assert.Equal(2, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidGrantorMiddleNames_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,ThisIsAVeryLongMiddleNameThatExceedsTheMaximumAllowedLengthOf75CharactersThisIsAVeryLongMiddleNameThatExceedsTheMaximumAllowedLengthOf75Characters,Doe,1HGCM82633A123456,2024-05-01,7,001 234 567,Test Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(1, upload.Submitted);
    Assert.Equal(1, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidGrantorLastName_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,,1HGCM82633A123456,2024-05-01,7,001 234 567,Test Company
Jane,Marie,ThisIsAVeryLongLastNameThatExceedsTheMaximumAllowedLengthOf35Characters,2FMDK48C87BA12345,2024-05-02,25,002 345 678,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(2, upload.Submitted);
    Assert.Equal(2, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidVIN_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,,2024-05-01,7,001 234 567,Test Company
Jane,Marie,Smith,123456789012345678,2024-05-02,25,002 345 678,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(2, upload.Submitted);
    Assert.Equal(2, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidRegistrationStartDate_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,,7,001 234 567,Test Company
John,,Doe,1HGCM82633A123456,2024-05-01 12:00:00,7,001 234 567,Test Company
Jane,Marie,Smith,2FMDK48C87BA12345,invalid-date,25,002 345 678,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(3, upload.Submitted);
    Assert.Equal(3, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidRegistrationDuration_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,2024-05-01,,001 234 567,Test Company
John,,Doe,1HGCM82633A123456,2024-05-01,5,001 234 567,Test Company
Jane,Marie,Smith,2FMDK48C87BA12345,2024-05-02,invalid-duration,002 345 678,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(3, upload.Submitted);
    Assert.Equal(3, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidSPGACN_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,2024-05-01,7,,Test Company
Jane,Marie,Smith,2FMDK48C87BA12345,2024-05-02,25,1234567890,Another Company
Jane,Marie,Smith,2FMDK48C87BA12345,2024-05-02,25,123a567890,Another Company"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(3, upload.Submitted);
    Assert.Equal(3, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  [Fact]
  public async Task ProcessUpload_WithInvalidSPGOrganizationName_ShouldMarkAsInvalid()
  {
    // Arrange
    using var context = await CreateTestContext();
    _mockDbContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(context);

    var uploadMessage = new PPSUploadMessage
    {
      FileName = "test.csv",
      ClientId = 1,
      Data = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,2024-05-01,7,001 234 567,
Jane,Marie,Smith,2FMDK48C87BA12345,2024-05-02,25,002 345 678,ThisIsAVeryLongOrganizationNameThatExceedsTheMaximumAllowedLengthOf75CharactersAndShouldBeMarkedAsInvalid"
    };

    // Act
    await _service.ProcessUpload(uploadMessage);

    // Assert
    var upload = await _assertContext.PersonalPropertySecurityUploads.FirstAsync();
    Assert.Equal(2, upload.Submitted);
    Assert.Equal(2, upload.Invalid);
    Assert.Equal(0, upload.Processed);

    await CleanDatabase(_assertContext);
  }

  private async Task<PPSRegisterDbContext> CreateTestContext()
  {

    var context = new PPSRegisterDbContext(_options);
    context.Database.EnsureCreated();

    // Add a test upload record
    context.PersonalPropertySecurityUploads.Add(new PersonalPropertySecurityUpload
    {
      FileName = "test.csv",
      ClientId = 1
    });
    await context.SaveChangesAsync();

    return context;
  }

  private static async Task CleanDatabase(PPSRegisterDbContext context)
  {
    context.ChangeTracker.Clear();
    context.PersonalPropertySecurities.RemoveRange(context.PersonalPropertySecurities);
    context.PersonalPropertySecurityUploads.RemoveRange(context.PersonalPropertySecurityUploads);
    await context.SaveChangesAsync();
  }
}
