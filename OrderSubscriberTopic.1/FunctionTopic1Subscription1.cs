using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace OrderSubscriberTopic._1;

public class FunctionTopic1Subscription1
{
    private readonly ILogger<FunctionTopic1Subscription1> _logger;

    public FunctionTopic1Subscription1(ILogger<FunctionTopic1Subscription1> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FunctionTopic1Subscription1))]
    public async Task Run(
        [ServiceBusTrigger("topic.1", "subscription.1", Connection = "SERVICEBUS_CONNECTION_STRING")]
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