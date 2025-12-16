// Middlewares/ConnectionLoggingMiddleware.cs

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using POS.Domain.Utilities;

namespace POS.Domain.Configs.Middlewares;

public class ConnectionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ConnectionLoggingMiddleware> _logger;

    public ConnectionLoggingMiddleware(RequestDelegate next, ILogger<ConnectionLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Chỉ log cho SignalR endpoints
        if (context.Request.Path.StartsWithSegments("/orderHub"))
        {
            var ipAddress = GetClientIp(context);
            var normalIp = CommonHelper.NormalizeIP(ipAddress);
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var connectionId = context.Request.Query["id"].ToString();
            
            _logger.LogInformation("[SignalR] Request to {Path} from IP: {IpAddress}, User-Agent: {UserAgent}, ConnectionId: {ConnectionId}",
                context.Request.Path, normalIp, userAgent, connectionId);
            
            // Thêm client IP vào response headers để debug
            context.Response.Headers.Append("X-Client-IP", ipAddress);
        }

        await _next(context);
    }

    private string GetClientIp(HttpContext context)
    {
        // X-Forwarded-For header (proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        // X-Real-IP header
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Remote IP address
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}