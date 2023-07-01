using System;

namespace Source.Components;

public class HMACManagerOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public string[] AdditionalContentHeaders { get; set; }
}

public class MessageContent
{
    public string? Header { get; set; }
    public string? Value { get; set; }
}