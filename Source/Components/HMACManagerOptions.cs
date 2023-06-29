using System;

namespace Source.Components;

public class HMACManagerOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
}