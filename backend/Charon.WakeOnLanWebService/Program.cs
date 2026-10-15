using System.Net;
using System.Net.NetworkInformation;
using Charon.WakeOnLanWebService.Data;
using Charon.WakeOnLanWebService.Dto;
using Charon.WakeOnLanWebService.Models;
using Charon.WakeOnLanWebService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

var services = builder.Services;
services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlite(connectionString));
services.AddOpenApi();
services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});
services.AddScoped<IDeviceManagementService, DeviceManagementService>();
services.AddSingleton<IWakeOnLanService, WakeOnLanService>();
services.AddSingleton<IPingService, PingService>();
services.AddLogging();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");

var devicesMapGroup = app.MapGroup("/devices");
devicesMapGroup.MapPost("/", async (
    [FromBody] AddDeviceRequest request,
    IDeviceManagementService deviceService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.MacAddress)
        || !PhysicalAddress.TryParse(request.MacAddress, out var macAddress))
    {
        return Results.Problem(new ProblemDetails
        {
            Title = "MAC address format error",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = $"Unable to parse MAC address '{request.MacAddress}'",
        });
    }

    IPAddress? ipAddress = null;
    if (!string.IsNullOrEmpty(request.IpAddress) && !IPAddress.TryParse(request.IpAddress, out ipAddress))
    {
        return Results.Problem(new ProblemDetails
        {
            Title = "IP address format error",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = $"Unable to parse IP address '{request.IpAddress}'",
        });
    }

    var deviceAdded = await deviceService.AddDevice(new NetworkDevice
    {
        MacAddress = macAddress,
        IpAddress = ipAddress,
        Title = request.Title,
    }, cancellationToken);

    if (deviceAdded)
    {
        return Results.Created();
    }

    return Results.BadRequest(new ProblemDetails
    {
        Title = "Device wasn't added",
    });
});
devicesMapGroup.MapGet("/", async (
    IDeviceManagementService deviceService,
    CancellationToken cancellationToken) =>
{
    var devices = deviceService.ListDevices(cancellationToken);
    var allDevices = await devices.ToArrayAsync(cancellationToken);
    
    return allDevices
        .Select(x => x.ToResponse())
        .ToArray();
});
devicesMapGroup.MapGet("/{mac}", async (
    [FromRoute] PhysicalAddress mac,
    IDeviceManagementService deviceService,
    CancellationToken cancellationToken) =>
{
    var device = await deviceService.GetDevice(mac, cancellationToken);
    if (device is null)
    {
        return Results.NotFound(new ProblemDetails
        {
            Title = "Device not found",
            Detail = $"Unable to find device with MAC address '{mac}'",
        });
    }
    
    return Results.Json(device.ToResponse());
});
devicesMapGroup.MapPost("/{mac}/wake", async (
    [FromRoute] PhysicalAddress mac,
    IDeviceManagementService deviceService,
    IWakeOnLanService wakeOnLanService,
    CancellationToken cancellationToken) =>
{
    var device = await deviceService.GetDevice(mac, cancellationToken);
    if (device is null)
    {
        return Results.NotFound(new ProblemDetails
        {
            Title = "Device not found",
            Detail = $"Unable to find device with MAC address '{mac}'",
        });
    }

    await wakeOnLanService.WakeOnLan(mac, cancellationToken);

    return Results.NoContent();
});
devicesMapGroup.MapPost("/{mac}/ping", async (
    [FromRoute] PhysicalAddress mac,
    IDeviceManagementService deviceService,
    IPingService pingService,
    CancellationToken cancellationToken) =>
{
    var device = await deviceService.GetDevice(mac, cancellationToken);
    if (device is null)
    {
        return Results.NotFound(new ProblemDetails
        {
            Title = "Device not found",
            Detail = $"Unable to find device with MAC address '{mac}'",
        });
    }
    
    if (device.IpAddress is null)
    {
        return Results.Json(new PingResponse
        {
            Success = false,
            Description = "IpIsNotSetForThisDevice",
            RoundTripTime = TimeSpan.Zero,
            RoundTripTimeMs = 0,
        });
    }

    var pingResult = await pingService.Ping(device.IpAddress, cancellationToken);
    
    return Results.Json(new PingResponse
    {
        Success = pingResult.Success,
        Description = pingResult.Description,
        RoundTripTime = pingResult.RoundTripTime,
        RoundTripTimeMs = pingResult.RoundTripTime.Milliseconds,
    });
});
devicesMapGroup.MapDelete("/{mac}", async (
    [FromRoute] PhysicalAddress mac,
    IDeviceManagementService deviceService,
    CancellationToken cancellationToken) =>
{
    var deleted = await deviceService.DeleteDevice(mac, cancellationToken);
    return deleted ? Results.NoContent() : Results.NotFound();
});
devicesMapGroup.MapPatch("/{mac}/title", async (
    [FromRoute] PhysicalAddress mac,
    [FromBody] UpdateDeviceTitleRequest request,
    IDeviceManagementService deviceService,
    CancellationToken cancellationToken) =>
{
    var updated = await deviceService.RenameDevice(mac, request.Title, cancellationToken);
    return updated ? Results.NoContent() : Results.NotFound();
});
devicesMapGroup.MapPatch("/{mac}/ip", async (
    [FromRoute] PhysicalAddress mac,
    [FromBody] UpdateDeviceIpRequest request,
    IDeviceManagementService deviceService,
    CancellationToken cancellationToken) =>
{
    IPAddress? ipAddress = null;
    if (!string.IsNullOrEmpty(request.IpAddress) && !IPAddress.TryParse(request.IpAddress, out ipAddress))
    {
        return Results.BadRequest(new ProblemDetails
        {
            Title = "IP address format error",
            Detail = $"Unable to parse IP address '{request.IpAddress}'",
        });
    }

    var updated = await deviceService.UpdateDeviceIp(mac, ipAddress, cancellationToken);
    return updated ? Results.NoContent() : Results.NotFound();
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync(); 
}

app.Run();