using LittleToolCollection;

var t = new TimeMeasurer();
var p = new DictionarySplitterByLength();

var dictFilePath = "dictionary.txt";

t.SetTimeToNow();
Console.WriteLine("Begin read dict file...");

await p.Load(dictFilePath);
t.PrintTimeUsedSinceLastTask();

p.GroupWordsByLength();
t.PrintTimeUsedSinceLastTask();

var report = p.GetDistributionReport();

await p.ExportByLength();
t.PrintTimeUsedSinceLastTask();
