using System.Net.NetworkInformation;

namespace Charon.WakeOnLanWebService.Services;

public interface IWakeOnLanService
{
    Task WakeOnLan(PhysicalAddress macAddress, CancellationToken cancellationToken);
}