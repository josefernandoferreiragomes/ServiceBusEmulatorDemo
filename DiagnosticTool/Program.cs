using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

var connectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
var topicName = "topic.1";
List<string> subscriptionNames = new() { "subscription.1", "subscription.2", "subscription.3" };
var queueName = "queue.1";
var client = new ServiceBusClient(connectionString);

// 🔹 Peek active messages
foreach (var subscriptionName in subscriptionNames)
{

    var receiver = client.CreateReceiver(topicName, subscriptionName);
    var activeMessages = await receiver.PeekMessagesAsync(maxMessages: 10);

    Console.WriteLine($"Peek {topicName} messages for subscription {subscriptionName}");

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
    var dlqMessages = await dlqReceiver.PeekMessagesAsync(maxMessages: 10);

    Console.WriteLine($"🔸  {topicName} {subscriptionName} Dead-letter Messages:");
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
foreach (var msg in queueDlqMessages)
{
    Console.WriteLine($"- MessageId: {msg.MessageId}");
    Console.WriteLine($"  Body: {msg.Body}");
    Console.WriteLine($"  DeadLetterReason: {msg.DeadLetterReason}");
    Console.WriteLine($"  DeadLetterErrorDescription: {msg.DeadLetterErrorDescription}");
}

Console.WriteLine();
Console.WriteLine("Do you want to try to consume messages from topic and queue? (y/n)");
var input = Console.ReadLine();
if (input?.ToLower() != "y")
{
   
    await client.DisposeAsync();
    Console.WriteLine("Exiting...");
    return;
}
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

Console.WriteLine();

await diagnosticQueueReceiver.DisposeAsync();
await client.DisposeAsync();
