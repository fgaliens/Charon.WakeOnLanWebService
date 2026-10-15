using System.Net;
using System.Net.NetworkInformation;

namespace Charon.WakeOnLanWebService.Models;

public record NetworkDevice
{
    public required PhysicalAddress MacAddress { get; init; }

    public IPAddress? IpAddress { get; init; }

    public string? Title { get; init; }
}