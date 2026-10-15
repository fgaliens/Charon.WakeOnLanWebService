using System.Net;
using System.Net.NetworkInformation;
using Charon.WakeOnLanWebService.Models;

namespace Charon.WakeOnLanWebService.Services;

public class PingService : IPingService
{
    public async Task<PingResult> Ping(IPAddress ipAddress, CancellationToken cancellationToken)
    {
        var pingTimeout = TimeSpan.FromSeconds(1);
        using var ping = new Ping();
        var response = await ping.SendPingAsync(
            ipAddress,
            pingTimeout,
            cancellationToken: cancellationToken);

        return new()
        {
            Success = response.Status == IPStatus.Success,
            Description = response.Status.ToString(),
            RoundTripTime = TimeSpan.FromMilliseconds(response.RoundtripTime),
        };
    }
}