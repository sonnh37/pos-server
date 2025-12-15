using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using POS.API.Extensions;
using POS.Domain.Configs;
using POS.Domain.Shared.Handler;
using POS.Domain.Utilities;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.LoadAssemblies();
// builder.Host.AddAppConfiguration();
// Add Cors
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureCors(builder.Configuration);
// Add serilog
builder.Host.AddSerilog(builder.Configuration);
// Exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.ConfigureContext(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.ConfigureBind();
builder.Services.AddServices();
builder.Services.AddRepositories();
// builder.Services.AddFluentValidation();

// builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerGen();

// -----------------app-------------------------

var app = builder.Build();
ValidatorHelper.ServiceProvider = app.Services;

app.UseExceptionHandler();

app.UseRouting();

app.UseCors(ServiceExtensions.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.Lifetime.ApplicationStarted.Register(() =>
{
    logger.LogInformation("[Successfully] Server started successfully and is listening for requests...");
});

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
}

app.UseHttpsRedirection();

app.Run();