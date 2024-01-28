using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace HtmlParser.ConsoleTest.Core.Mkb10;

internal class Mkb10Parser : IParser<IEnumerable<DiseasesClass>>
{
    public IEnumerable<DiseasesClass> Parse(IHtmlDocument document)
    {
        IEnumerable<IElement> elements = document.QuerySelectorAll("li")
            .Where(element => element.ParentElement != null
                              && element.ParentElement.ClassName != null
                              && element.ParentElement.ClassName.Contains("tm-list-additional-space list-codes"))
            .ToArray();

        IEnumerable<DiseasesClass> diseasesClasses = elements.Select(element =>
            {
                string[] rangeComponents = element.Children[0].TextContent.Split('-');
                CodesRange range = new(rangeComponents[0], rangeComponents[1]);

                if (element.Children[1] is not IHtmlAnchorElement anchorElement)
                    throw new ArgumentException();

                string title = anchorElement.TextContent;
                string url = anchorElement.PathName;

                const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

                string[] blocksCountComponents = element.Children[3]
                    .TextContent.Split(" ", SplitOptions);

                int blocksCount = int.Parse(blocksCountComponents[1]);

                string[] separator = ["\n", ":"];

                IList<string>? included = element.Children.Length < 5
                    ? null
                    : element.Children[5]
                        .TextContent.Split(separator, SplitOptions)
                        .Skip(1)
                        .ToList();

                IList<string>? excluded = element.Children.Length < 7
                    ? null
                    : element.Children[7]
                        .TextContent
                        .Split(separator, SplitOptions)
                        .Skip(1)
                        .ToList();

                return new DiseasesClass(range, title, blocksCount, url, included,
                    excluded);
            })
            .ToArray();

        return diseasesClasses;
    }
}

public class DiseasesClass
{
    public DiseasesClass(CodesRange range, string title, int blocksCount, string url, IList<string>? included = null, IList<string>? excluded = null)
    {
        Range = range;
        Title = title;
        BlocksCount = blocksCount;
        Url = url;
        Included = included;
        Excluded = excluded;
    }

    public string Url { get; }
    public CodesRange Range { get; }
    public string Title { get; }
    public int BlocksCount { get; }
    public IList<string>? Included { get; }
    public IList<string>? Excluded { get; }
}

public record CodesRange
{
    public CodesRange(string start, string end)
    {
        Start = start;
        End = end;
    }

    public string Start { get; }
    public string End { get; }
}