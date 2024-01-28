namespace HtmlParser.ConsoleTest.Core.Habra;

internal record HabraSettings(int StartPoint, int EndPoint) : IParserSettings
{
    public string BaseUrl { get; } = "https://habr.com/ru/feed";

    public string Prefix { get; } = "page{0}";
}