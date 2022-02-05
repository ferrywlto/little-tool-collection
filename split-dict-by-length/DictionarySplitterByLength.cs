// See https://aka.ms/new-console-template for more information

namespace LittleToolCollection;

public class DictionarySplitterByLength {
    private readonly bool performAnalysis;
    private readonly bool exportAnalysis;

    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
    private List<string> _words = new();
    private Dictionary<int, List<string>> _wordListByLength = new();
    private Dictionary<int, Dictionary<char, int>> _charDistributionByLength = new();
    private Dictionary<char, int> _globalCharDistribution = new();
    private int _totalWords;
    private int _totalChars;
    private int _maxWordLength;
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
        InitializeAfterLoad();
    }
    private void InitializeAfterLoad() {
        _totalWords = _words.Count;
        _totalChars = _words.Any() ? _words.Sum(s => s.Length) : 0;
        _maxWordLength = _words.Any() ? _words.Max(s => s.Length) : 0;
        _globalCharDistribution = InitializedCharDistribution();
        _charDistributionByLength = Enumerable.Range(1, _maxWordLength).ToDictionary(i => i, _ => InitializedCharDistribution());
        _wordListByLength = Enumerable.Range(1, _maxWordLength).ToDictionary(i => i, _ => new List<string>());
    }
    private Dictionary<char, int> InitializedCharDistribution() => Alphabet.ToDictionary(c => c, _ => 0);

    private static IEnumerable<string> CleanseInputList(IEnumerable<string> input) =>
        FilterDuplicate(input.Select(CleanseInput)).OrderBy(s => s);
    private static IEnumerable<string> FilterDuplicate(IEnumerable<string> input) => input.Distinct();
    private static string CleanseInput(string input) => input.Trim().ToLowerInvariant();
    public void GroupWordsByLength() {
        _words.ForEach(AddToWordListByLength);
    }
    private void AddToWordListByLength(string word) {
        _wordListByLength[word.Length].Add(word);
        CountCharDistributionByLength(word);
    }
    private void CountCharDistributionByLength(string word) {
        foreach (var c in word) { _charDistributionByLength[word.Length][c] += 1; }
    }
    public async Task ExportByLength(string outputFilePath = "") {
        foreach (var length in _wordListByLength.Keys) {
            var wordList = _wordListByLength[length];
            await File.WriteAllLinesAsync(Path.Combine(outputFilePath, $"len{length:d2}.txt"), wordList);
        }
    }

    public DistributionReport GetDistributionReport() {
        return new DistributionReport(GetGlobalCharDistribution(), GetGlobalLengthDistribution());
    }
    private GlobalLengthDistribution GetGlobalLengthDistribution() {
        return new GlobalLengthDistribution(_words.Count, GetLengthDistributions());
    }
    private LengthDistribution[] GetLengthDistributions() {
        return _wordListByLength.Keys.Select(GetLengthDistribution).ToArray();
    }
    private LengthDistribution GetLengthDistribution(int length) {
        var totalWordForLength = _wordListByLength[length].Count;
        var wordRatioByLength = totalWordForLength / (double)_totalWords;

        var charDistributions = GetCharDistributionsByLength(length);
        return new LengthDistribution(
            length,
            totalWordForLength,
            wordRatioByLength,
            charDistributions.Sum(d => d.Count),
            charDistributions);
    }
    private CharDistribution[] GetCharDistributionsByLength(int length) {
        var charDistByLength = _charDistributionByLength[length];
        var totalCharByLength = charDistByLength.Values.Sum();
        var charDistributions = charDistByLength.Select(
            pair => {
                var charPercentForLength = pair.Value / (double)totalCharByLength;
                return new CharDistribution(pair.Key, pair.Value, charPercentForLength);
            }).ToArray();

        return charDistributions;
    }
    private GlobalCharDistribution GetGlobalCharDistribution() {
        CountCharDistributionForAllWords();

        var sortedCharCountList = _globalCharDistribution.OrderBy(pair => pair.Key).ToList();

        var result = sortedCharCountList.Select(
            pair => {
                (var key, var value) = pair;
                var ratioOfCurrentChar = value / (double)_totalChars;
                return new CharDistribution(key, value, ratioOfCurrentChar);
            });

        return new GlobalCharDistribution(_totalChars, result.ToArray());
    }
    private void CountCharDistributionForAllWords() {
        _globalCharDistribution.Clear();
        _words.ForEach(CountCharDistribution);
    }
    private void CountCharDistribution(string word) {
        foreach (var c in word) {
            _globalCharDistribution[c] += 1;
        }
    }


    public record DistributionReport(GlobalCharDistribution DistByChar, GlobalLengthDistribution DistByLength);
    public record GlobalLengthDistribution(int TotalWord, LengthDistribution[] LengthDistributions);
    public record LengthDistribution(int Length, int Count, double Ratio, int TotalChar, CharDistribution[] CharDistributions) : GlobalCharDistribution(TotalChar, CharDistributions);
    public record GlobalCharDistribution(int TotalChar, CharDistribution[] CharDistributions);
    public record CharDistribution(char Letter, int Count, double Ratio);
}
