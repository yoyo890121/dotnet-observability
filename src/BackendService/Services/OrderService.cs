using Grpc.Core;
using Protos;

namespace BackendService.Services;

public class OrderService(ILogger<OrderService> logger) : Protos.OrderService.OrderServiceBase
{
    private static readonly Random s_random = new();

    public override async Task<OrderReply> GetOrder(GetOrderRequest request, ServerCallContext context)
    {
        logger.LogInformation("GetOrder called with Id={OrderId}", request.Id);

        // Simulate DB lookup latency
        await Task.Delay(s_random.Next(50, 200));

        return new OrderReply
        {
            Id = request.Id,
            Item = "Widget-" + request.Id,
            Quantity = s_random.Next(1, 10),
            Status = "Completed",
            CreatedAt = DateTime.UtcNow.AddDays(-s_random.Next(1, 30)).ToString("o")
        };
    }

    public override async Task<OrderReply> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
    {
        logger.LogInformation("PlaceOrder called with Item={Item}, Quantity={Quantity}", request.Item, request.Quantity);

        // Simulate processing latency
        await Task.Delay(s_random.Next(100, 300));

        var orderId = s_random.Next(1000, 9999);

        return new OrderReply
        {
            Id = orderId,
            Item = request.Item,
            Quantity = request.Quantity,
            Status = "Created",
            CreatedAt = DateTime.UtcNow.ToString("o")
        };
    }
}
