using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

//give time for SB to start and publisher to send the messages
Task.Delay(45000).Wait();

var composeConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");

// local kestrel connectiono string
var connectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
var topicName = "topic.1";
List<string> subscriptionNames = new() { "subscription.1", "subscription.2", "subscription.3" };
var queueName = "queue.1";
var client = new ServiceBusClient(string.IsNullOrEmpty(composeConnectionString) ? connectionString : composeConnectionString);

// 🔹 Peek active messages
foreach (var subscriptionName in subscriptionNames)
{

    var receiver = client.CreateReceiver(topicName, subscriptionName);
    var activeMessages = await receiver.PeekMessagesAsync(maxMessages: 100);

    Console.WriteLine($"Peek {topicName} messages for subscription {subscriptionName}");
    Console.WriteLine($"   Message Count: {activeMessages.Count}");
    Console.WriteLine($"Peek {topicName} {subscriptionName} Active Messages:");
    foreach (var msg in activeMessages)
    {
        Console.WriteLine($"- MessageId: {msg.MessageId}");
        Console.WriteLine($"  Body: {msg.Body}");
        Console.WriteLine($"  EnqueuedTime: {msg.EnqueuedTime}");
    }

    // 🔸 Peek dead-letter messages
    var dlqReceiver = client.CreateReceiver(topicName, subscriptionName, new ServiceBusReceiverOptions
    {
        SubQueue = SubQueue.DeadLetter
    });
    var dlqMessages = await dlqReceiver.PeekMessagesAsync(maxMessages: 100);

    Console.WriteLine($"🔸  {topicName} {subscriptionName} Dead-letter Messages:");
    Console.WriteLine($"   Message Count: {dlqMessages.Count}");
    foreach (var msg in dlqMessages)
    {
        Console.WriteLine($"- MessageId: {msg.MessageId}");
        Console.WriteLine($"  Body: {msg.Body}");
        Console.WriteLine($"  DeadLetterReason: {msg.DeadLetterReason}");
        Console.WriteLine($"  DeadLetterErrorDescription: {msg.DeadLetterErrorDescription}");
    }
    Console.WriteLine();
    await receiver.DisposeAsync();
    await dlqReceiver.DisposeAsync();

}

Console.WriteLine($"Peek queue {queueName} messages");
// 🔹 Peek active messages
var queuePeekReceiver = client.CreateReceiver(queueName);
var activeMessagesQueuePeek = await queuePeekReceiver.PeekMessagesAsync(maxMessages: 10);
Console.WriteLine($"   Message Count: {activeMessagesQueuePeek.Count}");
Console.WriteLine($"Peek 🔹{queueName} Active Messages:");
foreach (var msg in activeMessagesQueuePeek)
{
    Console.WriteLine($"- MessageId: {msg.MessageId}");
    Console.WriteLine($"  Body: {msg.Body}");
    Console.WriteLine($"  EnqueuedTime: {msg.EnqueuedTime}");
}

// 🔸 Peek dead-letter messages
var queueDlqReceiver = client.CreateReceiver(queueName, new ServiceBusReceiverOptions
{
    SubQueue = SubQueue.DeadLetter
});
var queueDlqMessages = await queueDlqReceiver.PeekMessagesAsync(maxMessages: 10);

Console.WriteLine($"🔸{queueName} Dead-letter Messages:");
Console.WriteLine($"   Message Count: {queueDlqMessages.Count}");
foreach (var msg in queueDlqMessages)
{
    Console.WriteLine($"- MessageId: {msg.MessageId}");
    Console.WriteLine($"  Body: {msg.Body}");
    Console.WriteLine($"  DeadLetterReason: {msg.DeadLetterReason}");
    Console.WriteLine($"  DeadLetterErrorDescription: {msg.DeadLetterErrorDescription}");
}

Console.WriteLine("Diagnostic completed");
await client.DisposeAsync();
