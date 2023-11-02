namespace HmacManagement.Components;

public class HmacResult
{
    public required string Policy { get; init; }
    public required string HeaderScheme { get; init; }
    public Hmac? Hmac { get; init; } = default;
    public bool IsSuccess { get; init; } = false;
    public DateTime DateGenerated { get; } = DateTime.UtcNow;
}