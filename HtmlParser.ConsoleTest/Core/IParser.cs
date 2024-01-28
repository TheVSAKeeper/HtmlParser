using AngleSharp.Html.Dom;

namespace HtmlParser.ConsoleTest.Core;

internal interface IParser<out T> where T : class
{
    T Parse(IHtmlDocument document);
}