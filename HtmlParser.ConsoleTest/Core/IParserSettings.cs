namespace HtmlParser.ConsoleTest.Core;

internal interface IParserSettings
{
    string BaseUrl { get; }

    string Prefix { get; }

    int StartPoint { get; }

    int EndPoint { get; }
}