namespace Charon.WakeOnLanWebService.Data.Models;

public record NetworkDeviceEntity
{
    public required byte[] MacAddress { get; set; }

    public byte[]? IpAddress { get; set; }

    public string? Title { get; set; }
}
