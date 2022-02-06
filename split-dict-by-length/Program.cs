using LittleToolCollection;

var t = new TimeMeasurer();
var p = new DictionarySplitterByLength();

var dictFilePath = "dictionary.txt";

t.SetTimeToNow();
Console.WriteLine("Begin read dict file...");

var words = await p.Load(dictFilePath);
t.PrintTimeUsedSinceLastTask();

await p.ExportByLength();
t.PrintTimeUsedSinceLastTask();

var a = new DistributionAnalyzer();
var report = a.AnalyzeDistribution(words);
t.PrintTimeUsedSinceLastTask();

var consoleReporter = new ConsoleOutput();
consoleReporter.Output(report);
t.PrintTimeUsedSinceLastTask();
