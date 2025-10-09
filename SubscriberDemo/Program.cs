using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

Task.Delay(50000).Wait();

// container connection string to be used when running in container
var composeConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");
// local kestrel connectiono string
var connectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
var client = new ServiceBusClient(string.IsNullOrEmpty(composeConnectionString) ? connectionString : composeConnectionString);

var topicName = "topic.1";
List<string> subscriptionNames = new() { "subscription.1", "subscription.2", "subscription.3" };
var queueName = "queue.1";

foreach (var subscriptionName in subscriptionNames)
{
    Console.WriteLine($"Try to consume messages from topic {topicName}, subscription {subscriptionName}");
    var diagnosticTopicReceiver = client.CreateReceiver(topicName, subscriptionName);
    var messageFromTopic = await diagnosticTopicReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));    
    if (messageFromTopic != null)
    {
        Console.WriteLine($"Message From Topic {topicName}, subscriber {subscriptionName}: message received");
        Console.WriteLine($"  {messageFromTopic.Body}");
    }

    Console.WriteLine();
    await diagnosticTopicReceiver.DisposeAsync();
}
var diagnosticQueueReceiver = client.CreateReceiver(queueName);
Console.WriteLine($"Message From Queue {queueName} receiver started");
var messageFromQueue = await diagnosticQueueReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
if (messageFromQueue != null)
{
    Console.WriteLine($"Message From Queue {queueName} message received...");
    Console.WriteLine($"  {messageFromQueue.Body}");
}

Console.WriteLine("Subscriber completed");

await diagnosticQueueReceiver.DisposeAsync();
await client.DisposeAsync();
