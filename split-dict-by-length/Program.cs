using LittleToolCollection;

var t = new TimeMeasurer();
t.SetTimeToNow();
Console.WriteLine("Begin...");

const string dictFilePath = "dictionary.txt";
var splitter = new DictionarySplitterByLength();
var words = await splitter.Load(dictFilePath);
t.PrintTimeUsedSinceLastTask();

await splitter.ExportByLength();
t.PrintTimeUsedSinceLastTask();

var analyzer = new DistributionAnalyzer();
var report = analyzer.AnalyzeDistribution(words);
t.PrintTimeUsedSinceLastTask();

var console = new ConsoleOutput();
console.Output(report);
t.PrintTimeUsedSinceLastTask();
