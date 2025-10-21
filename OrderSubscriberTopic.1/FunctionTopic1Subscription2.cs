using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace OrderSubscriberTopic._1;

public class FunctionTopic1Subscription2
{
    private const string topicName = "topic.1";
    private const string subscriptionName = "subscription.2";
    private readonly ILogger<FunctionTopic1Subscription2> _logger;
    
    static readonly Counter ConsumedMessages = Metrics.CreateCounter("subscriber_consumed_messages_total", "Messages consumed by subscriber");
    static readonly Counter BackendCalls = Metrics.CreateCounter("subscriber_backend_calls", "Backend calls made by subscriber");

    public FunctionTopic1Subscription2(ILogger<FunctionTopic1Subscription2> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FunctionTopic1Subscription2))]
    public async Task Run(
        [ServiceBusTrigger(topicName, subscriptionName, Connection = "SERVICEBUS_CONNECTION_STRING")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        await SubscriberMetrics.ProcessWithMetricsAsync(topicName, subscriptionName, async () =>
        {
            // actual message handling, e.g., call mock API
            await SubscriberMetrics.CallDependencyAsync("order-processor-backend", async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // HttpClient call to mock API
                    var response = await httpClient.GetAsync("http://order-processor-backend:8080/weatherforecast");
                    _logger.LogInformation("HTTP Response Status Code: {statusCode}", response.StatusCode);
                    
                    BackendCalls.Inc(1);
                    return response;
                }
            });
        });

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
        ConsumedMessages.Inc(1);
    }
}