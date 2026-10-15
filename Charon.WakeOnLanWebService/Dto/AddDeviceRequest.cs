namespace Charon.WakeOnLanWebService.Dto;

public record AddDeviceRequest
{
    public required string MacAddress { get; init; }
    
    public string? IpAddress { get; init; }

    public string? Title { get; init; }
}