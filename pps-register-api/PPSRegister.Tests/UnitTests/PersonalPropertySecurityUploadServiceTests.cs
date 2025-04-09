using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PPSRegister.Api.Services;
using PPSRegister.Data;
using PPSRegister.Data.Models;
using Xunit;

namespace PPSRegister.Tests.UnitTests;

public class PersonalPropertySecurityUploadServiceTests : IDisposable
{
    private readonly PPSRegisterDbContext _context;
    private readonly Mock<IAmazonSQS> _mockSqsClient;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly PersonalPropertySecurityUploadService _service;

    public PersonalPropertySecurityUploadServiceTests()
    {
        // Create in-memory database options
        var options = new DbContextOptionsBuilder<PPSRegisterDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PPSRegisterDbContext(options);

        _mockSqsClient = new Mock<IAmazonSQS>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Create a mock configuration section for AWS:SQS
        var mockConfigSection = new Mock<IConfigurationSection>();
        mockConfigSection.Setup(x => x.Value).Returns("https://sqs.test-region.amazonaws.com/test-queue");

        // Setup the configuration to return our mock section
        _mockConfiguration.Setup(x => x.GetSection("AWS:SQS:QueueUrl"))
            .Returns(mockConfigSection.Object);

        _service = new PersonalPropertySecurityUploadService(
            _context,
            _mockSqsClient.Object,
            _mockConfiguration.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task UploadFile_WithNullFile_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UploadFile(null!, 1));
    }

    [Fact]
    public async Task UploadFile_WithEmptyFile_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(), 0, 0, "test", "test.csv");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UploadFile(file, 1));
    }

    [Fact]
    public async Task UploadFile_WithNonCsvFile_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var content = "test content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, content.Length, "test", "test.txt");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UploadFile(file, 1));
    }

    [Fact]
    public async Task UploadFile_WithFileTooLarge_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var content = new string('a', 26 * 1024 * 1024); // 26MB
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, content.Length, "test", "test.csv");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UploadFile(file, 1));
    }

    [Fact]
    public async Task UploadFile_WithDuplicateFileName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var content = "test content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, content.Length, "test", "test.csv");

        await _context.PersonalPropertySecurityUploads.AddAsync(new PersonalPropertySecurityUpload
        {
            FileName = "test.csv",
            ClientId = 1
        });
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UploadFile(file, 1));

        await CleanDatabase(_context);
    }

    [Fact]
    public async Task UploadFile_WithInvalidCsvFormat_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var content = "Invalid,Header,Row";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, content.Length, "test", "test.csv");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UploadFile(file, 1));
    }

    [Fact]
    public async Task UploadFile_WithValidFile_ShouldSucceed()
    {
        // Arrange
        var content = @"Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name
John,,Doe,1HGCM82633A123456,2024-05-01,7,001 234 567,Test Company";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, content.Length, "test", "test.csv");

        _mockSqsClient.Setup(c => c.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse());

        // Act
        var result = await _service.UploadFile(file, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.csv", result.FileName);
        Assert.Equal(1, result.ClientId);

        // Verify the upload was saved to the database
        var savedUpload = await _context.PersonalPropertySecurityUploads.FirstOrDefaultAsync(u => u.FileName == "test.csv");
        Assert.NotNull(savedUpload);
        Assert.Equal(1, savedUpload.ClientId);

        _mockSqsClient.Verify(c => c.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);

        await CleanDatabase(_context);
    }

    [Fact]
    public async Task GetUploads_ShouldReturnUploadsForClient()
    {
        // Arrange
        var testUploads = new List<PersonalPropertySecurityUpload>
        {
            new() { Id = 1, FileName = "test1.csv", ClientId = 1 },
            new() { Id = 2, FileName = "test2.csv", ClientId = 1 }
        };

        await _context.PersonalPropertySecurityUploads.AddRangeAsync(testUploads);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUploads(1);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, u => Assert.Equal(1, u.ClientId));

        await CleanDatabase(_context);
    }

    private static async Task CleanDatabase(PPSRegisterDbContext context)
    {
        context.ChangeTracker.Clear();
        context.PersonalPropertySecurities.RemoveRange(context.PersonalPropertySecurities);
        context.PersonalPropertySecurityUploads.RemoveRange(context.PersonalPropertySecurityUploads);
        await context.SaveChangesAsync();
    }
}