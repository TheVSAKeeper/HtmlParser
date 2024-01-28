using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace HtmlParser.ConsoleTest.Core.Mkb10;

internal class Mkb10Parser : IParser<IEnumerable<DiseasesClass>>
{
    private const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    public IEnumerable<DiseasesClass> Parse(IHtmlDocument document)
    {
        IEnumerable<DiseasesClass> diseasesClasses = document
            .QuerySelectorAll("li")
            .Where(element => element.ParentElement is { ClassName: not null }
                              && element.ParentElement.ClassName.Contains("tm-list-additional-space list-codes"))
            .Select(element =>
            {
                CodesRange range = ParseCodesRange(element);

                ParseTitle(element, out string title, out string url);

                int blocksCount = ParseBlocksCount(element);

                IList<string>? included = ParseDescription(element, 5);
                IList<string>? excluded = ParseDescription(element, 7);

                return new DiseasesClass(range, title, blocksCount, url, included, excluded);
            })
            .ToArray();

        return diseasesClasses;
    }

    private static CodesRange ParseCodesRange(IParentNode element)
    {
        string[] rangeComponents = element.Children[0].TextContent.Split('-');
        CodesRange range = new(rangeComponents[0], rangeComponents[1]);
        return range;
    }

    private static void ParseTitle(IParentNode element, out string title, out string url)
    {
        if (element.Children[1] is not IHtmlAnchorElement anchorElement)
            throw new ArgumentException(nameof(anchorElement));

        title = anchorElement.TextContent;
        url = anchorElement.PathName;
    }

    private static int ParseBlocksCount(IParentNode element)
    {
        string[] blocksCountComponents = element.Children[3]
            .TextContent.Split(" ", SplitOptions);

        int blocksCount = int.Parse(blocksCountComponents[1]);
        return blocksCount;
    }

    private IList<string>? ParseDescription(IParentNode element, int index)
    {
        IList<string>? description = element.Children.Length < index
            ? null
            : element.Children[index]
                .TextContent
                .Replace("Исключены: ", string.Empty)
                .Replace("Включены: ", string.Empty)
                .Split(["\n"], SplitOptions)
                .Skip(1)
                .ToList();

        return description;
    }
}