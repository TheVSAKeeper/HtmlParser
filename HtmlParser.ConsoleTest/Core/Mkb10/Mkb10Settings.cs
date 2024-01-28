namespace HtmlParser.ConsoleTest.Core.Mkb10;

internal record Mkb10Settings(int StartPoint, int EndPoint) : IParserSettings
{
    public string BaseUrl { get; } = "https://mkb10.su";

    public string Prefix { get; }
}