using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HtmlParser.ConsoleTest.Core;
using HtmlParser.ConsoleTest.Core.Mkb10;

namespace HtmlParser.ConsoleTest;

internal static class Program
{
    private static readonly ParserWorker<IEnumerable<DiseasesClass>> Parser;

    static Program()
    {
        Parser = new ParserWorker<IEnumerable<DiseasesClass>>(new Mkb10Parser());

        Parser.OnCompleted += Parser_OnCompleted;
        Parser.OnNewData += Parser_OnNewData;
    }

    private static void Parser_OnNewData(object arg1, IEnumerable<DiseasesClass> enumerable)
    {
        JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        };

        foreach (DiseasesClass diseasesClass in enumerable)
            Console.WriteLine(JsonSerializer.Serialize(diseasesClass, options));
    }

    private static void Parser_OnCompleted(object obj)
    {
        Console.WriteLine("All works done!");
    }

    public static void Main()
    {
        Parser.Settings = new Mkb10Settings();
        Parser.Start();

        Console.ReadLine();
        Parser.Abort();
    }
}