using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

//give time for SB to start
Task.Delay(40000).Wait();

ServiceBusClient client;

ServiceBusSender sender;
ServiceBusSender senderQueue;

const int numOfMessages = 3;

// container connection string to be used when running in container
var composeConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");
//local kestrel connectiono string
string connectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
client = new ServiceBusClient(string.IsNullOrEmpty(composeConnectionString) ? connectionString : composeConnectionString);

string topicName = "topic.1";

sender = client.CreateSender(topicName);

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();


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
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}
try
{
    await sender.SendMessagesAsync(messageBatch1);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {topicName}. topic, for subscription.1 filter");
}
finally
{
    messageBatch1.Dispose();
}

//send to subscription 2
using ServiceBusMessageBatch messageBatch2 = await sender.CreateMessageBatchAsync();
for (int i = 1; i <= numOfMessages; i++)
{
    var message2 = new ServiceBusMessage("Hello Subscription 2!")
    {
        ContentType = "text/plain"
    };

    message2.ApplicationProperties["prop1"] = "value1";

    if (!messageBatch2.TryAddMessage(message2))
    {
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
    messageBatch2.Dispose();    
}

//send to subscription 3
for (int i = 1; i <= numOfMessages; i++)
{
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {topicName}. topic, for subscription.3");
    Console.WriteLine("(no filter, as it does not match the previous subscriptions' filters)");
}
finally
{ 
    messageBatch.Dispose();
    await sender.DisposeAsync();
}

// Send To Queue
string queueName = "queue.1";
senderQueue = client.CreateSender(queueName);
using ServiceBusMessageBatch messageBatchQueue = await senderQueue.CreateMessageBatchAsync();

for (int i = 1; i <= numOfMessages; i++)
{
    if (!messageBatchQueue.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    await senderQueue.SendMessagesAsync(messageBatchQueue);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {queueName} queue.");
}
finally
{
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