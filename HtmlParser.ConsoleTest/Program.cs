using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HtmlParser.ConsoleTest.Core;
using HtmlParser.ConsoleTest.Core.Mkb10;

namespace HtmlParser.ConsoleTest;

internal static class Program
{
    private static readonly ParserWorker<IEnumerable<DiseasesClass>> Parser;
    private static readonly JsonSerializerOptions SerializerOptions;
    private static readonly ConcurrentBag<DiseasesClass> DiseasesClasses;
    private static readonly string Path;

    static Program()
    {
        Parser = new ParserWorker<IEnumerable<DiseasesClass>>(new Mkb10Parser());
        DiseasesClasses = new ConcurrentBag<DiseasesClass>();
        new object();
        Path = @$"{Directory.GetCurrentDirectory()}\data\{DateTime.Now:[yyyy-MM-dd]HH-mm-ss}.json";

        SerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        };

        Parser.OnCompleted += Parser_OnCompleted;
        Parser.OnNewData += Parser_OnNewData;
    }

    private static void Parser_OnNewData(object context, IEnumerable<DiseasesClass> diseasesClasses)
    {
        foreach (DiseasesClass diseasesClass in diseasesClasses)
        {
            string serialize = JsonSerializer.Serialize(diseasesClass, SerializerOptions);

            DiseasesClasses.Add(diseasesClass);

            Console.WriteLine(serialize);
        }
    }

    private static void Parser_OnCompleted(object context)
    {
        Console.WriteLine("All works done!");
    }

    public static void Main()
    {
        Parser.Settings = new Mkb10Settings();
        Parser.Start();

        Console.ReadLine();
        Parser.Abort();

        File.WriteAllText(Path, JsonSerializer.Serialize(DiseasesClasses, SerializerOptions));
        Console.WriteLine($"File {Path} saved");
    }
}