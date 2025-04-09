using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using PPSRegister.Data.Models;

namespace PPSRegister.PPSUploader.Workers;

public class PPSUploadWorker(IAmazonSQS _sqsClient, IConfiguration _configuration, ILogger<PPSUploadWorker> _logger) : BackgroundService
{
    private readonly string _queueUrl = _configuration.GetValue<string>("AWS:SQS:QueueUrl") ?? "";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PPS Uploader Service started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 5
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest, stoppingToken);

                if (response.Messages != null && response.Messages.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        _logger.LogInformation("Received message: {MessageId} with body: {MessageBody}", message.MessageId, message.Body);

                        var uploadMessage = JsonSerializer.Deserialize<PPSUploadMessage>(message.Body);

                        // await _ppsUploadProcessingService.ProcessUpload(uploadMessage);

                        // After successful processing, delete the message
                        await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                        _logger.LogInformation("Deleted message: {MessageId}", message.MessageId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing SQS messages");
            }
        }
        _logger.LogInformation("SQS Worker stopping at: {time}", DateTimeOffset.Now);
    }
}
