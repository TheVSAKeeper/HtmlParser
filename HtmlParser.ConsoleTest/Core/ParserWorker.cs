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
        if (_parserSettings == null)
            throw new ArgumentNullException(nameof(_parserSettings));

        if (_loader == null)
            throw new ArgumentNullException(nameof(_loader));

        for (int i = _parserSettings.StartPoint; i <= _parserSettings.EndPoint; i++)
        {
            if (IsActive == false)
            {
                OnCompleted?.Invoke(this);
                return;
            }

            string? source = await _loader.GetSourceByPageId(i);
            AngleSharp.Html.Parser.HtmlParser domParser = new();

            if (source == null)
                continue;

            IHtmlDocument document = await domParser.ParseDocumentAsync(source);

            T result = Parser.Parse(document);

            OnNewData?.Invoke(this, result);
        }

        OnCompleted?.Invoke(this);
        IsActive = false;
    }
}