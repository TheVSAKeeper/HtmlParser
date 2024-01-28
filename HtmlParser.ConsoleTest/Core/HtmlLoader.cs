using System.Net;

namespace HtmlParser.ConsoleTest.Core;

internal class HtmlLoader(IParserSettings settings)
{
    private readonly HttpClient _client = new();

    private readonly string _url = $"{settings.BaseUrl}/"
                                   + (string.IsNullOrWhiteSpace(settings.Prefix)
                                       ? string.Empty
                                       : $"{settings.Prefix}/");

    public async Task<string?> GetSourceByPageId(int id)
    {
        string currentUrl = string.Format(_url, id);
        HttpResponseMessage response = await _client.GetAsync(currentUrl).ConfigureAwait(false);

        string? source = null;

        if (response.StatusCode == HttpStatusCode.OK)
            source = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return source;
    }
}