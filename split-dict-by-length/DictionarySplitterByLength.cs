// See https://aka.ms/new-console-template for more information

namespace LittleToolCollection;

public class DictionarySplitterByLength {
    public static async Task Main(string[] args) {
        var t = new TimeMeasurer();
        var p = new DictionarySplitterByLength();

        var dictFilePath = "dictionary.txt";

        t.SetTimeToNow();
        Console.WriteLine("Begin read dict file...");

        await p.Load(dictFilePath);
        t.PrintTimeUsedSinceLastTask();

        p.GroupWordsByLength();
        t.PrintTimeUsedSinceLastTask();

        await p.ExportByLength();
        t.PrintTimeUsedSinceLastTask();
    }


    private readonly bool performAnalysis;
    private readonly bool exportAnalysis;

    private List<string> _words = new();
    private Dictionary<int, List<string>> _wordListByLength = new();
    private Dictionary<int, Dictionary<char, int>> _charCountByLength = new();
    private Dictionary<char, int> _globalCharCount = new();

    public DictionarySplitterByLength(bool performAnalysis = false,
                                      bool exportAnalysis = false
    ) {
        this.performAnalysis = performAnalysis;
        this.exportAnalysis = exportAnalysis;
    }

    public async Task Load(string inputFilePath) {
        if (!File.Exists(inputFilePath)) throw new FileNotFoundException();

        var loadedWords = await File.ReadAllLinesAsync(inputFilePath);
        _words = CleanseInputList(loadedWords).ToList();
    }

    private static IEnumerable<string> CleanseInputList(IEnumerable<string> input) =>
        FilterDuplicate(input.Select(CleanseInput)).OrderBy(s => s);
    private static IEnumerable<string> FilterDuplicate(IEnumerable<string> input) => input.Distinct();
    private static string CleanseInput(string input) => input.Trim().ToLowerInvariant();
    public void GroupWordsByLength() {
        _words.ForEach(AddToWordListByLength);
    }
    private void AddToWordListByLength(string word) {
        InitWordListByLengthIfNotExist(word.Length);
        _wordListByLength[word.Length].Add(word);
    }
    private void InitWordListByLengthIfNotExist(int length) {
        if (!_wordListByLength.ContainsKey(length))
            _wordListByLength.Add(length, new List<string>());
    }
    public async Task ExportByLength(string outputFilePath = "") {
        foreach (var length in _wordListByLength.Keys) {
            var wordList = _wordListByLength[length];
            await File.WriteAllLinesAsync(Path.Combine(outputFilePath, $"len{length:d2}.txt"), wordList);
        }
    }

    private void IncreaseCharCountByChar(char glyph) {
        InitCharCountByCharEntryIfNotExist(glyph);
        _globalCharCount[glyph] += 1;
    }
    private void InitCharCountByCharEntryIfNotExist(char glyph) {
        if(!_globalCharCount.ContainsKey(glyph))
            _globalCharCount.Add(glyph, 0);
    }
    private void InitCharCountByLengthIfNotExist(int length) {
        if(!_charCountByLength.ContainsKey(length))
            _charCountByLength.Add(length, new Dictionary<char, int>());
    }
    private void InitCharCountEntryIfNotExist(Dictionary<char, int> dictionary, char glyph) {
        if(!dictionary.ContainsKey(glyph))
            dictionary.Add(glyph, 0);
    }
    private void IncreaseCharCountByLength(Dictionary<char, int> dictionary, char glyph) {
        InitCharCountEntryIfNotExist(dictionary, glyph);
        dictionary[glyph]+= 1;
    }
    private void IncreaseCharCountByLength(char glyph, int length) {
        IncreaseCharCountByLength(_charCountByLength[length], glyph);
    }

    private async Task GroupAndCount() {
        foreach (var word in _words) {
            AddToWordListByLength(word);

            InitCharCountByLengthIfNotExist(word.Length);

            for (var i = 0; i < word.Length; i += 1) {
                var letter = word[i];
                IncreaseCharCountByChar(letter);
                IncreaseCharCountByLength(letter, word.Length);
            }
        }
    }
    private void GenerateReport() {

        var sortedWordListByLength = _wordListByLength.OrderBy(p => p.Key).ToList();
        var totalWords = sortedWordListByLength.Sum(s => s.Value.Count);

        foreach (var wordListByLength in sortedWordListByLength) {
            var totalCharForLength = wordListByLength.Value.Sum(s => s.Length);

            var wordRatioByLength = wordListByLength.Value.Count / (double)totalWords;
            Console.WriteLine($"LengthDistribution {{ Length = {wordListByLength.Key}, Count = {wordListByLength.Value.Count} ({wordRatioByLength:P5}), Total Char = {totalCharForLength}");


            var charDist = _charCountByLength[wordListByLength.Key];
            var charDistList = charDist.OrderBy(pair => pair.Key).ToList();

            foreach ((var key, var value) in charDistList) {
                var charPercentForLength = value / (double)totalCharForLength;
                Console.WriteLine($"\t|- CharDistribution {{ Char = '{key}', Count = {value} ({charPercentForLength:P5})}}");
            }
        }

        var globalCharDist = GetGlobalCharDistribution();
    }

    private GlobalCharDistribution GetGlobalCharDistribution() {
        var sortedCharCountList = _globalCharCount.OrderBy(pair => pair.Key).ToList();
        var totalChar = _words.Sum(s => s.Length);

        var result = sortedCharCountList.Select(
            pair => {
                (var key, var value) = pair;
                var ratioOfCurrentChar = value / (double)totalChar;
                return new CharDistribution(key, value, ratioOfCurrentChar);
            });

        return new GlobalCharDistribution(totalChar, result.ToArray());
    }



    private record DistributionReport(GlobalCharDistribution distByChar, LengthDistribution[] distByLength);
    private record GlobalLengthDistribution(int TotalWord, LengthDistribution[] LengthDistributions);
    private record LengthDistribution(int Length, int Count, double Ratio, int TotalChar, CharDistribution[] CharDistributions) : GlobalCharDistribution(TotalChar, CharDistributions);
    private record GlobalCharDistribution(int TotalChar, CharDistribution[] CharDistributions);
    private record CharDistribution(char Letter, int Count, double Ratio);
}
