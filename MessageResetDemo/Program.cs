using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

//give time for SB to start and publisher to send the messages
//Task.Delay(45000).Wait();

// container connection string to be used when running in container
var composeConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");

// local kestrel connectiono string
var connectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
var topicName = "topic.1";
List<string> subscriptionNames = new() { "subscription.1", "subscription.2", "subscription.3" };
var queueName = "queue.1";
var client = new ServiceBusClient(string.IsNullOrEmpty(composeConnectionString) ? connectionString : composeConnectionString);

// 🔹 Delete active messages
foreach (var subscriptionName in subscriptionNames)
{

    var receiver = client.CreateReceiver(
        topicName, 
        subscriptionName, 
        new ServiceBusReceiverOptions() 
        {
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        }
    );
    var activeMessages = await receiver.ReceiveMessagesAsync(maxMessages: 100);

    Console.WriteLine($"Receive and Delete from Topic {topicName} messages for subscription {subscriptionName}");
    Console.WriteLine($"   Message Count: {activeMessages.Count}");
    Console.WriteLine($"Receive and Delete from {topicName} {subscriptionName} Active Messages:");
    foreach (var msg in activeMessages)
    {
        Console.WriteLine($"- MessageId: {msg.MessageId}");
        Console.WriteLine($"  Body: {msg.Body}");
        Console.WriteLine($"  EnqueuedTime: {msg.EnqueuedTime}");
    }

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
    var dlqMessages = await dlqReceiver.ReceiveMessagesAsync(maxMessages: 100);

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
    await receiver.DisposeAsync();
    await dlqReceiver.DisposeAsync();

}

Console.WriteLine($"Receive and Delete from Queue {queueName} messages");
// 🔹 Receive and Delete from active messages
var queueReceiver = client.CreateReceiver(
    queueName, 
    new ServiceBusReceiverOptions() 
    { 
        ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete 
    }
);
var activeMessagesQueue = await queueReceiver.ReceiveMessagesAsync(maxMessages: 100);
Console.WriteLine($"Receive and Delete from 🔹 Queue {queueName} Active Messages:");
Console.WriteLine($"   Message Count: {activeMessagesQueue.Count}");
foreach (var msg in activeMessagesQueue)
{
    Console.WriteLine($"- MessageId: {msg.MessageId}");
    Console.WriteLine($"  Body: {msg.Body}");
    Console.WriteLine($"  EnqueuedTime: {msg.EnqueuedTime}");
}

// 🔸 Receive and Delete from dead-letter messages
var queueDlqReceiver = client.CreateReceiver(
    queueName, 
    new ServiceBusReceiverOptions
    {
        SubQueue = SubQueue.DeadLetter,
        ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
    }
);
var queueDlqMessages = await queueDlqReceiver.ReceiveMessagesAsync(maxMessages: 100);

Console.WriteLine($"🔸Receive and Delete from Queue {queueName} Dead-letter Messages:");
Console.WriteLine($"   Message Count: {queueDlqMessages.Count}");
foreach (var msg in queueDlqMessages)
{
    Console.WriteLine($"- MessageId: {msg.MessageId}");
    Console.WriteLine($"  Body: {msg.Body}");
    Console.WriteLine($"  DeadLetterReason: {msg.DeadLetterReason}");
    Console.WriteLine($"  DeadLetterErrorDescription: {msg.DeadLetterErrorDescription}");
}

Console.WriteLine("Message Reset completed");
await queueDlqReceiver.DisposeAsync();
await client.DisposeAsync();
