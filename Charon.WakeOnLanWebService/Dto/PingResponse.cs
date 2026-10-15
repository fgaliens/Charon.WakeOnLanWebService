namespace Charon.WakeOnLanWebService.Dto;

public record PingResponse
{
    public required bool Success { get; init; }
    public required string Description { get; init; }
    public required TimeSpan RoundTripTime { get; init; }
    public required int RoundTripTimeMs { get; init; }
}