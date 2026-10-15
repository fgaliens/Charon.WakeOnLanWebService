namespace Charon.WakeOnLanWebService.Dto;

public record DeviceResponse
{
    public required string MacAddress { get; init; }

    public required string? IpAddress { get; init; }

    public required string Title { get; init; }
}