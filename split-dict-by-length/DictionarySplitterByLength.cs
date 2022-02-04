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

        await p.GroupAndCount();
        t.PrintTimeUsedSinceLastTask();

        await p.ExportByLength();
        t.PrintTimeUsedSinceLastTask();
    }


    private readonly bool performAnalysis;
    private readonly bool exportAnalysis;

    private List<string> _words = new();
    private Dictionary<int, List<string>> _dictWordListByLength = new();
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

    private void IncreaseCharCountByChar(char glyph) {
        InitCharCountByCharEntryIfNotExist(glyph);
        _globalCharCount[glyph] += 1;
    }
    private void InitCharCountByCharEntryIfNotExist(char glyph) {
        if(!_globalCharCount.ContainsKey(glyph))
            _globalCharCount.Add(glyph, 0);
    }
    private void AddToWordListByLength(string word) {
        InitWordListByLengthIfNotExist(word.Length);
        _dictWordListByLength[word.Length].Add(word);
    }
    private void InitWordListByLengthIfNotExist(int length) {
        if (!_dictWordListByLength.ContainsKey(length))
            _dictWordListByLength.Add(length, new List<string>());
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

    private static IEnumerable<string> CleanseInputList(IReadOnlyList<string> input) {
        var output = new string[input.Count];

        for (var i = 0; i < input.Count; i += 1) {
            output[i] = CleanseInput(input[i]);
        }

        output = FilterDuplicate(output);

        Array.Sort(output);

        return output;
    }

    private static string CleanseInput(string input) {
        return input.Trim().ToLowerInvariant();
    }
    private static string[] FilterDuplicate(IEnumerable<string> input) {
        return input.Distinct().ToArray();
    }

    private void Summary() {

        foreach (var length in _charCountByLength.Keys) {
            var d = _charCountByLength[length];

            foreach (var letter in d.Keys) {
                if (!_globalCharCount.ContainsKey(letter))
                    _globalCharCount.Add(letter, 1);
                else
                    _globalCharCount[letter] += d[letter];
            }
        }

        var lengthDistList = _dictWordListByLength.OrderBy(p => p.Key).ToList();
        var totalWords = _dictWordListByLength.Sum(s => s.Value.Count);
        foreach (var lengthDist in lengthDistList) {
            var totalCharForLength = lengthDist.Value.Sum(s => s.Length);
            var wordPercent = lengthDist.Value.Count / (double)totalWords;
            Console.WriteLine($"LengthDistribution {{ Length = {lengthDist.Key}, Count = {lengthDist.Value.Count} ({wordPercent:P5}), Total Char = {totalCharForLength}");


            var charDist = _charCountByLength[lengthDist.Key];
            var charDistList = charDist.OrderBy(pair => pair.Key).ToList();

            foreach ((var key, var value) in charDistList) {
                var charPercentForLength = value / (double)totalCharForLength;
                Console.WriteLine($"\t|- CharDistribution {{ Char = '{key}', Count = {value} ({charPercentForLength:P5})}}");
            }
            Console.WriteLine("}");
//            var lengthDistribution = new LengthDistribution(length, _dictByLength[length].Count, charDistList.ToArray());

        }

        var _dictByCharList = _globalCharCount.OrderBy(pair => pair.Key).ToList();
        var totalCharForChar = _dictByCharList.Sum(s => s.Value);
        foreach ((var key, var value) in _dictByCharList) {
            var charPercent = value / (double)totalCharForChar;
            Console.WriteLine($"FullCharDistribution {{ Char = '{key}', Count = {value} ({charPercent:P5})}}");
        }
        Console.WriteLine($"Total Char = {totalCharForChar}");
//        var fullCharDistList = _dictByChar.Keys.Select(letter => new CharDistribution(letter, _dictByChar[letter])).ToList();
//        Console.WriteLine(fullCharDistList);
    }

    async Task ExportByLength(string outputFilePath = "") {
        foreach (var length in _dictWordListByLength.Keys) {
            var wordList = _dictWordListByLength[length];
            await File.WriteAllLinesAsync(Path.Combine(outputFilePath, $"len{length:d2}.txt"), wordList);
        }
    }

    private record LengthDistribution(int Length, int Count, CharDistribution[] CharDistributions);
    private record CharDistribution(char Letter, int Count);
}
