namespace Charon.WakeOnLanWebService.Dto;

public record UpdateDeviceIpRequest
{
    public string? IpAddress { get; init; }
}