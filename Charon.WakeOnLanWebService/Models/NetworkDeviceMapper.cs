using System.Net;
using System.Net.NetworkInformation;
using Charon.WakeOnLanWebService.Data.Models;
using Charon.WakeOnLanWebService.Dto;

namespace Charon.WakeOnLanWebService.Models;

public static class NetworkDeviceMapper
{
    extension(NetworkDevice networkDevice)
    {
        public NetworkDeviceEntity ToEntity()
        {
            return new()
            {
                MacAddress = networkDevice.MacAddress.GetAddressBytes(),
                IpAddress = networkDevice.IpAddress?.GetAddressBytes(),
                Title = networkDevice.Title,
            };
        }
    }
    
    extension(NetworkDevice networkDevice)
    {
        public DeviceResponse ToResponse()
        {
            var title = networkDevice.Title ?? $"Device {networkDevice.MacAddress}";
            
            return new()
            {
                MacAddress = networkDevice.MacAddress.ToString(),
                IpAddress = networkDevice.IpAddress?.ToString(),
                Title = title,
            };
        }
    }
    
    extension(NetworkDeviceEntity networkDeviceEntity)
    {
        public NetworkDevice ToModel()
        {
            return new()
            {
                MacAddress = new PhysicalAddress(networkDeviceEntity.MacAddress),
                IpAddress = networkDeviceEntity.IpAddress is not null 
                    ? new IPAddress(networkDeviceEntity.IpAddress) 
                    : null,
                Title = networkDeviceEntity.Title,
            };
        }
    }
}