namespace OrderProcessingDemo.Models
{
    public interface IOrderProcessor
    {
        void ProcessOrder(string order);
    }

    public class MessageProcessingService
    {
        public void ProcessMessage(string message)
        {
            // Simulate message processing
            System.Console.WriteLine($"Processing message: {message}");
        }
    }
}
