using System.Net;
using Charon.WakeOnLanWebService.Models;

namespace Charon.WakeOnLanWebService.Services;

public interface IPingService
{
    public Task<PingResult> Ping(IPAddress ipAddress, CancellationToken cancellationToken);
}