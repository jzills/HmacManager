namespace HmacManager.Components;

public class HmacResult
{
    public Hmac? Hmac { get; init; } = default;
    public bool IsSuccess { get; init; } = false;
}