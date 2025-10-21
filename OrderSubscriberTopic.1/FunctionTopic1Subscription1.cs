using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderSubscriberTopic._1;

public class FunctionTopic1Subscription1
{
    private readonly ILogger<FunctionTopic1Subscription1> _logger;
    static readonly Counter ConsumedMessages = Metrics.CreateCounter("subscriber_consumed_messages_total", "Messages consumed by subscriber");
    static readonly Counter BackendCalls = Metrics.CreateCounter("subscriber_backend_calls", "Backend calls made by subscriber");
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

        //HttpClient httpClient = new HttpClient();
        //var response = await httpClient.GetAsync("http://order-processor-backend:8080");
        //_logger.LogInformation("HTTP Response Status Code: {statusCode}", response.StatusCode);

        await SubscriberMetrics.ProcessWithMetricsAsync("topic.1", "subscription.1", async () =>
        {
            // actual message handling, e.g., call mock API
            await SubscriberMetrics.CallDependencyAsync("mock-api", async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // HttpClient call to mock API
                    var response = await httpClient.GetAsync("http://order-processor-backend:8080");
                    _logger.LogInformation("HTTP Response Status Code: {statusCode}", response.StatusCode);
                    response.EnsureSuccessStatusCode();
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