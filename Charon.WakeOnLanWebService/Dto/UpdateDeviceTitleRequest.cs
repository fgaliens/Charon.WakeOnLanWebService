namespace Charon.WakeOnLanWebService.Dto;

public record UpdateDeviceTitleRequest
{
    public string? Title { get; init; }
}