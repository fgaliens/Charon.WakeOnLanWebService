using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Charon.WakeOnLanWebService.Data;
using Charon.WakeOnLanWebService.Models;
using Microsoft.EntityFrameworkCore;

namespace Charon.WakeOnLanWebService.Services;

public class DeviceManagementService(
    AppDbContext dbContext,
    ILogger<DeviceManagementService> logger) : IDeviceManagementService
{
    public async Task<bool> AddDevice(NetworkDevice networkDevice, CancellationToken cancellationToken)
    {
        try
        {
            dbContext.Devices.Add(networkDevice.ToEntity());
            await dbContext.SaveChangesAsync(cancellationToken);
            
            logger.LogInformation("Device added: {Device}", networkDevice);
            
            return true;
        }
        catch (DbUpdateException e)
        {
            logger.LogWarning(e, "Error adding device");
            return false;
        }
    }
    
    public async IAsyncEnumerable<NetworkDevice> ListDevices([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing devices");
        
        var entityStream = dbContext
            .Devices
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var entity in entityStream.WithCancellation(cancellationToken))
        {
            var networkDevice = entity.ToModel();
            logger.LogDebug("Devices: {Device}", networkDevice);
            yield return networkDevice;
        }
    }

    public async Task<NetworkDevice?> GetDevice(PhysicalAddress macAddress, CancellationToken cancellationToken)
    {
        var macAddressBytes = macAddress.GetAddressBytes();
        var deviceEntity = await dbContext.Devices.FirstOrDefaultAsync(
            x => x.MacAddress == macAddressBytes,
            cancellationToken);
        
        var networkDevice = deviceEntity?.ToModel();
        logger.LogInformation("Got device with MAC address {Mac}: '{Device}'", macAddress, networkDevice);
        return networkDevice;
    }
    
    public async Task<bool> DeleteDevice(PhysicalAddress macAddress, CancellationToken cancellationToken)
    {
        var macAddressBytes = macAddress.GetAddressBytes();
        var entitiesRemoved = await dbContext.Devices
            .Where(x => x.MacAddress == macAddressBytes)
            .ExecuteDeleteAsync(cancellationToken);
        
        logger.LogInformation("Removing device with MAC address: {Mac}. Removed: {Count} item(s)", macAddress, entitiesRemoved);
        
        return entitiesRemoved > 0;
    }
    
    public async Task<bool> RenameDevice(
        PhysicalAddress macAddress,
        string? newTitle,
        CancellationToken cancellationToken)
    {
        var macAddressBytes = macAddress.GetAddressBytes();
        var entitiesUpdated = await dbContext.Devices
            .Where(x => x.MacAddress == macAddressBytes)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.Title, newTitle), 
                cancellationToken);
        
        logger.LogInformation("Changing name for device with MAC address {Mac} to '{NewName}'. {Count} entity(ies) affected", 
            macAddress, newTitle, entitiesUpdated);
        
        return entitiesUpdated > 0;
    }

    public async Task<bool> UpdateDeviceIp(
        PhysicalAddress macAddress, 
        IPAddress? newIpAddress, 
        CancellationToken cancellationToken)
    {
        var macAddressBytes = macAddress.GetAddressBytes();
        var ipAddressBytes = newIpAddress?.GetAddressBytes();
        var entitiesUpdated = await dbContext.Devices
            .Where(x => x.MacAddress == macAddressBytes)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.IpAddress, ipAddressBytes), 
                cancellationToken);
        
        logger.LogInformation("Changing IP for device with MAC address {Mac} to '{IP}'. {Count} entity(ies) affected", 
            macAddress, newIpAddress, entitiesUpdated);
        
        return entitiesUpdated > 0;
    }
}