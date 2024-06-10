using HmacManager.Headers;

namespace HmacManager.Mvc;

internal class HeaderSchemeConfigurationSection : HeaderScheme
{
    public HeaderSchemeConfigurationSection(string? name) : base(name)
    {
    }

    public new List<Header>? Headers { get; set; }
}