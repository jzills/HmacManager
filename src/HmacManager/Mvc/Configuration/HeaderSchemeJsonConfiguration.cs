using HmacManager.Headers;

namespace HmacManager.Mvc;

internal class HeaderSchemeJsonConfiguration
{
    public string? Name { get; set; }
    public List<Header>? Headers { get; set; }
}