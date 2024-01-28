namespace HtmlParser.ConsoleTest.Core.Mkb10;

internal record Mkb10Settings : IParserSettings
{
    public string BaseUrl { get; } = "https://mkb10.su";
    public string Prefix { get; }
    public int StartPoint { get; } = 1;
    public int EndPoint { get; } = 1;
}