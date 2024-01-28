using System.Diagnostics;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace HtmlParser.ConsoleTest.Core;

internal class ParserWorker<T>(IParser<T> parser)
    where T : class
{
    private HtmlLoader? _loader;
    private IParserSettings? _parserSettings;

    public ParserWorker(IParser<T> parser, IParserSettings parserSettings) : this(parser)
    {
        _parserSettings = parserSettings;
    }

    public IParser<T> Parser { get; } = parser;

    public IParserSettings? Settings
    {
        get => _parserSettings;
        set
        {
            _parserSettings = value;
            _loader = new HtmlLoader(value);
        }
    }

    public bool IsActive { get; private set; }

    public event Action<object, T>? OnNewData;
    public event Action<object>? OnCompleted;

    public void Start()
    {
        IsActive = true;
        Worker();
    }

    public void Abort()
    {
        IsActive = false;
    }

    private async void Worker()
    {
        Debug.Assert(_parserSettings != null, nameof(_parserSettings) + " != null");

        for (int i = _parserSettings.StartPoint; i <= _parserSettings.EndPoint; i++)
        {
            if (!IsActive)
            {
                OnCompleted?.Invoke(this);
                return;
            }

            Debug.Assert(_loader != null, nameof(_loader) + " != null");
            string? source = await _loader.GetSourceByPageId(i);
            AngleSharp.Html.Parser.HtmlParser domParser = new();

            Debug.Assert(source != null, nameof(source) + " != null");
            IHtmlDocument document = await domParser.ParseDocumentAsync(source);

            T result = Parser.Parse(document);

            OnNewData?.Invoke(this, result);
        }

        OnCompleted?.Invoke(this);
        IsActive = false;
    }
}