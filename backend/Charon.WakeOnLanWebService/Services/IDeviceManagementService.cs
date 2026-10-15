using System.Net;
using System.Net.NetworkInformation;
using Charon.WakeOnLanWebService.Models;

namespace Charon.WakeOnLanWebService.Services;

public interface IDeviceManagementService
{
    Task<bool> AddDevice(NetworkDevice networkDevice, CancellationToken cancellationToken);
    
    IAsyncEnumerable<NetworkDevice> ListDevices(CancellationToken cancellationToken);

    Task<NetworkDevice?> GetDevice(PhysicalAddress macAddress, CancellationToken cancellationToken);
    
    Task<bool> DeleteDevice(PhysicalAddress macAddress, CancellationToken cancellationToken);

    Task<bool> RenameDevice(
        PhysicalAddress macAddress,
        string? newTitle,
        CancellationToken cancellationToken);
    
    Task<bool> UpdateDeviceIp(
        PhysicalAddress macAddress,
        IPAddress? newIpAddress,
        CancellationToken cancellationToken);
}