using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace OrderSubscriberQueue._1;

public class FunctionQueue1
{
    private readonly ILogger<FunctionQueue1> _logger;

    public FunctionQueue1(ILogger<FunctionQueue1> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FunctionQueue1))]
    public async Task Run(
        [ServiceBusTrigger("queue.1", Connection = "SERVICEBUS_CONNECTION_STRING")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}