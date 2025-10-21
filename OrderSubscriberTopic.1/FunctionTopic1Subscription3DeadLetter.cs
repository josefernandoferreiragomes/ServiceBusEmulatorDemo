using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Prometheus;
using System.Threading.Tasks;

namespace OrderSubscriberTopic._1;

public class FunctionTopic1Subscription3DeadLetter
{
    private const string topicName = "topic.1";
    private readonly ILogger<FunctionTopic1Subscription3DeadLetter> _logger;

    //static readonly Counter DeadLetterMessages = Metrics.CreateCounter(
    //    "subscriber_dead_letter_messages_total",
    //    "Messages moved to dead-letter queue"
    //);

    public FunctionTopic1Subscription3DeadLetter(ILogger<FunctionTopic1Subscription3DeadLetter> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FunctionTopic1Subscription3DeadLetter))]
    public async Task Run(
        [ServiceBusTrigger(topicName, "subscription.3/$DeadLetterQueue", Connection = "SERVICEBUS_CONNECTION_STRING")]
        ServiceBusReceivedMessage message)
    {
        _logger.LogWarning("Dead-lettered Message ID: {id}", message.MessageId);
        _logger.LogWarning("Dead-letter Reason: {reason}", message.DeadLetterReason);
        _logger.LogWarning("Dead-letter Error Description: {description}", message.DeadLetterErrorDescription);
        _logger.LogWarning("Message Body: {body}", message.Body);

        SubscriberMetrics.DeadletterMessages.Inc(1);

        // Optional: Persist or alert on dead-lettered messages
        await Task.CompletedTask;
    }
}