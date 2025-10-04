using Azure.Messaging.ServiceBus;

// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusProcessor processor;

var topicName = "topic.1";
var subscriptionName = "subscription.1";

//client = new ServiceBusClient(
//    "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;");
client = new ServiceBusClient(
    "Endpoint=sb://127.0.0.1:5672/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;");

processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

try
{
    //Task.Delay(5000).Wait();
    processor.ProcessMessageAsync += async args =>
    {
        Console.WriteLine($"Received: {args.Message.Body}");
        await args.CompleteMessageAsync(args.Message);
    };

    processor.ProcessErrorAsync += args =>
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("🔴 Error occurred:");
        Console.WriteLine($"- Error Source: {args.ErrorSource}");
        Console.WriteLine($"- Entity Path: {args.EntityPath}");
        Console.WriteLine($"- FullyQualifiedNamespace: {args.FullyQualifiedNamespace}");
        Console.WriteLine($"- Exception Type: {args.Exception.GetType().Name}");
        Console.WriteLine($"- Message: {args.Exception.Message}");
        Console.WriteLine($"- StackTrace: {args.Exception.StackTrace}");
        Console.ResetColor();

        return Task.CompletedTask;
    };

    await processor.StartProcessingAsync();

    Console.WriteLine("Subscriber running. Press Ctrl+C to exit.");
    await Task.Delay(-1);
   
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();