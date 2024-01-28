namespace HtmlParser.ConsoleTest;

public class DiseasesClass(CodesRange range, string title, int blocksCount, string url, IList<string>? included = null, IList<string>? excluded = null)
{
    public string Url { get; } = url;
    public CodesRange Range { get; } = range;
    public string Title { get; } = title;
    public int BlocksCount { get; } = blocksCount;
    public IList<string>? Included { get; } = included;
    public IList<string>? Excluded { get; } = excluded;
}