using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Charon.WakeOnLanWebService.Services;

public class WakeOnLanService : IWakeOnLanService, IDisposable
{
    private readonly ILogger<WakeOnLanService> _logger;
    private readonly UdpClient _udpClient;
    private readonly int[] _wakeOnLanPorts = [9, 7];

    public WakeOnLanService(ILogger<WakeOnLanService> logger)
    {
        _logger = logger;
        
        _udpClient = new UdpClient();
        _udpClient.EnableBroadcast = true;
    }
    
    public async Task WakeOnLan(PhysicalAddress macAddress, CancellationToken cancellationToken)
    {
        var magicPacket = CreateMagicPacket(macAddress);
        foreach (var port in _wakeOnLanPorts)
        {
            var endpoint = new IPEndPoint(IPAddress.Broadcast, port);
            await _udpClient.SendAsync(magicPacket, endpoint, cancellationToken);
        }
        
        _logger.LogInformation("Wake on LAN packet sent for {Device}", macAddress);
    }

    private static byte[] CreateMagicPacket(PhysicalAddress macAddress)
    {
        const int magicPacketSize = 6 + 16 * 6; 
        
        var magicPacket = new byte[magicPacketSize];
        Array.Fill(magicPacket, (byte)0xFF, 0, 6);
        
        var macAddressBytes = macAddress.GetAddressBytes();
        for (var i = 0; i < 16; i++)
        {
            Buffer.BlockCopy(macAddressBytes, 0, magicPacket, 6 + i * 6, 6);
        }

        return magicPacket;
    }

    public void Dispose()
    {
        _udpClient.Dispose();
    }
}