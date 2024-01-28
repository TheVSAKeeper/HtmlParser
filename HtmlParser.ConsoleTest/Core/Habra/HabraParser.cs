using AngleSharp.Html.Dom;

namespace HtmlParser.ConsoleTest.Core.Habra;

internal class HabraParser : IParser<IEnumerable<string>>
{
    public IEnumerable<string> Parse(IHtmlDocument document)
    {
        return document.QuerySelectorAll("a")
            .Where(item => item.ClassName?.Contains("tm-title__link") == true)
            .Select(item => item.TextContent);
    }
}