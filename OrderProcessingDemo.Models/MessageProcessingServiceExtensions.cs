namespace OrderProcessingDemo.Models
{
    public static class MessageProcessingServiceExtensions
    {
        public static void ProcessMessageWithHeaders(
            this MessageProcessingService service, 
            string message, 
            IDictionary<string, string> headers)
        {
            // Example: Custom header logic
            if (headers.TryGetValue("X-Custom-Header", out var headerValue))
            {
                System.Console.WriteLine($"Custom Header: {headerValue}");
            }

            // Call the original processing logic
            service.ProcessMessage(message);
        }
    }
}
