// Hubs/OrderHub.cs

using Microsoft.AspNetCore.SignalR;
using POS.Domain.Contracts.Services;
using POS.Domain.Models.CQRS.Queries.Orders;
using POS.Domain.Models.Results;
using POS.Domain.Models.Results.Bases;

namespace POS.API.Hubs;

public class OrderHub : Hub
{
    private readonly ILogger<OrderHub> _logger;
    private readonly IOrderService _orderService;

    public OrderHub(
        ILogger<OrderHub> logger,
        IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    // Client gọi method này để lấy tất cả orders
    public async Task SendAllOrders()
    {
        var clientIp = GetClientIp();
        _logger.LogInformation("[SignalR] Client {ConnectionId} (IP: {ClientIp}) requested all orders", 
            Context.ConnectionId, clientIp);

        try
        {
            // Gọi service để lấy orders
            var result = await _orderService.GetAll(new OrderGetAllQuery
            {
                Pagination =
                {
                    IsPagingEnabled = false,
                }
            });
            
            var pagedList = result.Data as IPagedList<OrderResult>;
            var orders = pagedList?.Results ?? new List<OrderResult>();

            _logger.LogInformation("[SignalR] Sending {OrderCount} orders to client {ConnectionId}", 
                pagedList?.TotalItemCount, Context.ConnectionId);

            // 🔴 QUAN TRỌNG: Chỉ gửi cho Caller (client đang request)
            await Clients.Caller.SendAsync("ReceiveAllOrders", orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SignalR] Error getting orders from service");
            // Gửi empty array cho Caller
            await Clients.Caller.SendAsync("ReceiveAllOrders", new List<OrderResult>());
        }
    }

    // 🟢 Khi có đơn hàng mới, broadcast cho TẤT CẢ clients
    public async Task BroadcastNewOrder(OrderResult order)
    {
        var clientIp = GetClientIp();
        _logger.LogInformation("[SignalR] Broadcasting new order #{OrderNumber} to ALL clients from {ConnectionId} (IP: {ClientIp})", 
            order.OrderNumber, Context.ConnectionId, clientIp);
        
        // 🟢 QUAN TRỌNG: Gửi cho TẤT CẢ clients đang kết nối
        await Clients.All.SendAsync("ReceiveNewOrder", order);
    }
    // Khi có đơn hàng mới được tạo từ OrdersController
    public async Task SendNewOrder(OrderResult order)
    {
        var clientIp = GetClientIp();
        _logger.LogInformation("[SignalR] Broadcasting new order #{OrderNumber} to all clients",
            order.OrderNumber);

        await Clients.All.SendAsync("ReceiveNewOrder", order);
    }

    // Test ping
    public async Task<string> Ping()
    {
        var clientIp = GetClientIp();
        _logger.LogInformation("[SignalR] Ping from {ConnectionId} (IP: {ClientIp})",
            Context.ConnectionId, clientIp);

        return $"Pong at {DateTime.UtcNow:HH:mm:ss}";
    }

    // Report client info
    public async Task<string> ReportClientInfo(string? clientInfo = null)
    {
        var clientIp = GetClientIp();

        _logger.LogInformation(
            "[SignalR] Client info - ConnectionId: {ConnectionId}, IP: {ClientIp}, ClientReported: {ReportedInfo}",
            Context.ConnectionId, clientIp, clientInfo ?? "Not reported");

        return $"Server detected IP: {clientIp}";
    }

    private string? GetClientIp()
    {
        try
        {
            var context = Context.GetHttpContext();
            if (context == null) return "Unknown";

            // Lấy IP từ X-Forwarded-For (nếu có proxy)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',').FirstOrDefault()?.Trim();
            }

            // Lấy IP trực tiếp
            return context.Connection.RemoteIpAddress?.ToString();
        }
        catch
        {
            return "Error getting IP";
        }
    }

    public override async Task OnConnectedAsync()
    {
        var clientIp = GetClientIp();

        _logger.LogInformation("[SignalR] 🟢 Client connected: {ConnectionId} from IP: {ClientIp}",
            Context.ConnectionId, clientIp);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var clientIp = GetClientIp();

        if (exception != null)
        {
            _logger.LogWarning(exception,
                "[SignalR] 🔴 Client disconnected with error: {ConnectionId} from IP: {ClientIp}",
                Context.ConnectionId, clientIp);
        }
        else
        {
            _logger.LogInformation("[SignalR] 🔴 Client disconnected: {ConnectionId} from IP: {ClientIp}",
                Context.ConnectionId, clientIp);
        }

        await base.OnDisconnectedAsync(exception);
    }
}