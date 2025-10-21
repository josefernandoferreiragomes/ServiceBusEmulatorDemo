using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

//Task.Delay(50000).Wait();

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
    var subscriberTopicReceiver = client.CreateReceiver(
        topicName, 
        subscriptionName, 
        new ServiceBusReceiverOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        }
    );
    var activeMessages = await subscriberTopicReceiver.ReceiveMessagesAsync(maxMessages: 1);

    Console.WriteLine($"Receive and Delete from Topic {topicName} messages for subscription {subscriptionName}");
    Console.WriteLine($"   Message Count: {activeMessages.Count}");
    Console.WriteLine($"Receive and Delete from {topicName} {subscriptionName} Active Messages:");
    foreach (var msg in activeMessages)
    {
        Console.WriteLine($"- MessageId: {msg.MessageId}");
        Console.WriteLine($"  Body: {msg.Body}");
        Console.WriteLine($"  EnqueuedTime: {msg.EnqueuedTime}");
    }
    
    var remainingActiveMessages = await subscriberTopicReceiver.ReceiveMessagesAsync(maxMessages: 100);
    if (remainingActiveMessages.Count == 0)
    {
        Console.WriteLine("   No more active messages");
    }
    else
    {
        Console.WriteLine($"   There are still {remainingActiveMessages.Count} active messages remaining");
    }

    Console.WriteLine();

    // 🔸 Receive and Delete from dead-letter messages
    var dlqReceiver = client.CreateReceiver(
        topicName,
        subscriptionName,
        new ServiceBusReceiverOptions
        {
            SubQueue = SubQueue.DeadLetter,
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        }
    );
    var dlqMessages = await dlqReceiver.ReceiveMessagesAsync(maxMessages: 1);

    Console.WriteLine($"🔸Receive and Delete from Topic {topicName} {subscriptionName} Dead-letter Messages:");
    Console.WriteLine($"   Message Count: {dlqMessages.Count}");
    foreach (var msg in dlqMessages)
    {
        Console.WriteLine($"- MessageId: {msg.MessageId}");
        Console.WriteLine($"  Body: {msg.Body}");
        Console.WriteLine($"  DeadLetterReason: {msg.DeadLetterReason}");
        Console.WriteLine($"  DeadLetterErrorDescription: {msg.DeadLetterErrorDescription}");
    }
    Console.WriteLine();

    var remainingDlqMessages = await dlqReceiver.ReceiveMessagesAsync(maxMessages: 100);
    if (remainingDlqMessages.Count == 0)
    {
        Console.WriteLine("   No more active DLQ messages");
    }
    else
    {
        Console.WriteLine($"   There are still {remainingDlqMessages.Count} active DLQ messages remaining");
    }

    await subscriberTopicReceiver.DisposeAsync();
    await dlqReceiver.DisposeAsync();
}

Console.WriteLine();

Console.WriteLine($"Receive and Delete from Queue {queueName} messages");
// 🔹 Receive and Delete from active messages
var queueReceiver = client.CreateReceiver(
    queueName,
    new ServiceBusReceiverOptions()
    {
        ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
    }
);
var activeMessagesQueue = await queueReceiver.ReceiveMessagesAsync(maxMessages: 1);
Console.WriteLine($"Receive and Delete from 🔹 Queue {queueName} Active Messages:");
Console.WriteLine($"   Message Count: {activeMessagesQueue.Count}");
foreach (var msg in activeMessagesQueue)
{
    Console.WriteLine($"- MessageId: {msg.MessageId}");
    Console.WriteLine($"  Body: {msg.Body}");
    Console.WriteLine($"  EnqueuedTime: {msg.EnqueuedTime}");
}

var remainingMessagesQueue = await queueReceiver.ReceiveMessagesAsync(maxMessages: 100);
if (remainingMessagesQueue.Count == 0)
{
    Console.WriteLine("   No more active messages");
}
else
{
    Console.WriteLine($"   There are still {remainingMessagesQueue.Count} active messages remaining");
}

Console.WriteLine();

// 🔸 Receive and Delete from dead-letter messages
var queueDlqReceiver = client.CreateReceiver(
    queueName,
    new ServiceBusReceiverOptions
    {
        SubQueue = SubQueue.DeadLetter,
        ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
    }
);
var queueDlqMessages = await queueDlqReceiver.ReceiveMessagesAsync(maxMessages: 1);

Console.WriteLine($"🔸Receive and Delete from Queue {queueName} Dead-letter Messages:");
Console.WriteLine($"   Message Count: {queueDlqMessages.Count}");
foreach (var msg in queueDlqMessages)
{
    Console.WriteLine($"- MessageId: {msg.MessageId}");
    Console.WriteLine($"  Body: {msg.Body}");
    Console.WriteLine($"  DeadLetterReason: {msg.DeadLetterReason}");
    Console.WriteLine($"  DeadLetterErrorDescription: {msg.DeadLetterErrorDescription}");
}

var remainingQueueDlqMessages = await queueReceiver.ReceiveMessagesAsync(maxMessages: 100);
if (remainingQueueDlqMessages.Count == 0)
{
    Console.WriteLine("   No more active DLQ messages");
}
else
{
    Console.WriteLine($"   There are still {remainingQueueDlqMessages.Count} active DLQ messages remaining");
}

Console.WriteLine();

Console.WriteLine("Message Consumption completed");
await queueDlqReceiver.DisposeAsync();
await client.DisposeAsync();