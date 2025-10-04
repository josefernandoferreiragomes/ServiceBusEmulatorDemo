using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

//give time for SB to start, publisher to send the messages, and diagnostic to show them
Task.Delay(60000).Wait();

var composeConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");
// local kestrel connectiono string
var connectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
var topicName = "topic.1";
List<string> subscriptionNames = new() { "subscription.1", "subscription.2", "subscription.3" };
var queueName = "queue.1";
var client = new ServiceBusClient(string.IsNullOrEmpty(composeConnectionString) ? connectionString : composeConnectionString);

foreach (var subscriptionName in subscriptionNames)
{
    Console.WriteLine("Try to consume messages...");
    var diagnosticTopicReceiver = client.CreateReceiver(topicName, subscriptionName);
    Console.WriteLine("messageFromTopic receiver started");
    var messageFromTopic = await diagnosticTopicReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
    Console.WriteLine("messageFromTopic message received...");
    if (messageFromTopic != null)
    {
        Console.WriteLine($"messageFromTopic Received: {messageFromTopic.Body}");
    }

    Console.WriteLine();
    await diagnosticTopicReceiver.DisposeAsync();
}
var diagnosticQueueReceiver = client.CreateReceiver(queueName);
Console.WriteLine("messageFromQueue receiver started");
var messageFromQueue = await diagnosticQueueReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
Console.WriteLine("messageFromQueue message received...");
if (messageFromQueue != null)
{
    Console.WriteLine($" messageFromQueue Received: {messageFromQueue.Body}");
}

Console.WriteLine("Subscriber completed");

await diagnosticQueueReceiver.DisposeAsync();
await client.DisposeAsync();
