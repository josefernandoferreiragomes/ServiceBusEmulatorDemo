using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;
ServiceBusSender senderQueue;

// number of messages to be sent to the queue
const int numOfMessages = 3;

var composeConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");
//local kestrel connectiono string
string connectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
client = new ServiceBusClient(string.IsNullOrEmpty(composeConnectionString) ? connectionString : composeConnectionString);

string topicName = "topic.1";

sender = client.CreateSender(topicName);

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

//give time for SB to start
Task.Delay(25000).Wait();

//send to subscription 3
for (int i = 1; i <= numOfMessages; i++)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {topicName}. topic, for subscription.3 (no filter)");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    //await sender.DisposeAsync();
    //await client.DisposeAsync();
    messageBatch.Dispose();
}

//send to subscription 2
using ServiceBusMessageBatch messageBatch2 = await sender.CreateMessageBatchAsync();
for (int i = 1; i <= numOfMessages; i++)
{
    // try adding a message to the batch
    var message2 = new ServiceBusMessage("Hello Subscription 2!")
    {
        ContentType = "text/plain"
    };

    // Add user-defined properties
    message2.ApplicationProperties["prop1"] = "value1";

    if (!messageBatch2.TryAddMessage(message2))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessagesAsync(messageBatch2);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {topicName}. topic, for subscription.2 filter");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    //await sender.DisposeAsync();
    //await client.DisposeAsync();
    messageBatch2.Dispose();
}

//send to subscription 1
using ServiceBusMessageBatch messageBatch1 = await sender.CreateMessageBatchAsync();
for (int i = 1; i <= numOfMessages; i++)
{
    var message1 = new ServiceBusMessage("Hello Subscription 1!")
    {
        ContentType = "application/text",
        CorrelationId = "id1",
        Subject = "subject1",            // maps to "Label" in config.json
        MessageId = "msgid1",
        ReplyTo = "someQueue",
        ReplyToSessionId = "sessionId",
        SessionId = "session1",
        To = "xyz"
    };

    if (!messageBatch1.TryAddMessage(message1))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}
try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessagesAsync(messageBatch1);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {topicName}. topic, for subscription.1 filter");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    //await client.DisposeAsync();
    messageBatch1.Dispose();
}
string queueName = "queue.1";

senderQueue = client.CreateSender(queueName);

// create a batch 
using ServiceBusMessageBatch messageBatchQueue = await senderQueue.CreateMessageBatchAsync();

Task.Delay(15000).Wait();

for (int i = 1; i <= numOfMessages; i++)
{
    // try adding a message to the batch
    if (!messageBatchQueue.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await senderQueue.SendMessagesAsync(messageBatchQueue);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {queueName} queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await senderQueue.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Publisher completed");
bool isContainer = Environment.GetEnvironmentVariable("RUNNING_IN_CONTAINER") == "true";
if (!isContainer)
{
    Console.WriteLine("Press any key to end the application");
    Console.ReadKey();
}