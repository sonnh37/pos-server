using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using POS.API.Extensions;
using POS.API.Hubs;
// Thêm namespace này
using POS.Domain.Configs;
using POS.Domain.Configs.Middlewares;
using POS.Domain.Configs.Slugify;
using POS.Domain.Shared.Handler;
using POS.Domain.Utilities;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.LoadAssemblies();

builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureCors(builder.Configuration);

// Configure SignalR với detailed logging
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
}).AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configure logging cho SignalR
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug); // Bật debug log
});

// Add serilog
builder.Host.AddSerilog(builder.Configuration);

// Exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(
        new RouteTokenTransformerConvention(new SlugifyParameterTransformer())
    );
});

builder.Services.ConfigureContext(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.ConfigureBind();
builder.Services.AddServices();
builder.Services.AddRepositories();

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerGen();

// -----------------app-------------------------

var app = builder.Build();
ValidatorHelper.ServiceProvider = app.Services;

app.UseExceptionHandler();

app.UseMiddleware<ConnectionLoggingMiddleware>();
// app.UseMiddleware<AuthenticationMiddleware>();
app.UseRouting();

app.UseCors(ServiceExtensions.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

// Chỉ dùng AuthenticationMiddleware nếu thực sự cần
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        // Log tất cả request trong dev
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogDebug("[HTTP] {Method} {Path} from {Ip}", 
            context.Request.Method, 
            context.Request.Path,
            context.Connection.RemoteIpAddress);
        
        await next();
    });
}

app.UseMiddleware<AuthenticationMiddleware>();
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Log khi app started
app.Lifetime.ApplicationStarted.Register(() =>
{
    var urls = app.Urls;
    logger.LogInformation("[Successfully] Server started successfully!");
    logger.LogInformation("[URLs] Listening on: {Urls}", string.Join(", ", urls));
    logger.LogInformation("[SignalR] Hub available at: {Url}/orderHub", urls.FirstOrDefault()?.Replace("https://", "").Replace("http://", ""));
});

// Log khi SignalR hub mapped
app.Lifetime.ApplicationStarted.Register(() =>
{
    logger.LogInformation("[SignalR] Hub mapped at /orderHub");
});

app.SeedDatabase();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
    });
    
    // Thêm endpoint để test SignalR connection
   
}

app.UseHttpsRedirection();

// Map SignalR hub với logging
app.MapHub<OrderHub>("/orderHub").WithDisplayName("OrderHub");

app.Run();