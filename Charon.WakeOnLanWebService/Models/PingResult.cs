namespace Charon.WakeOnLanWebService.Models;

public record PingResult
{
    public required bool Success { get; init; }
    public required string Description { get; init; }
    public required TimeSpan RoundTripTime { get; init; }
}