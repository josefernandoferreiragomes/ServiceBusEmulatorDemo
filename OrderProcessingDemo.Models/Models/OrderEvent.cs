namespace OrderProcessingDemo.Models;

public class OrderEvent
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; } = 0;
    public string ProductId { get; set; } = string.Empty;
}
