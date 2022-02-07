// See https://aka.ms/new-console-template for more information

namespace LittleToolCollection;

public class DictionarySplitterByLength {
    private List<string> _words = new();
    private Dictionary<int, List<string>> _wordListByLength = new();

    public async Task ExportByLength(string outputFilePath = "") {
        foreach (var length in _wordListByLength.Keys) {
            var wordList = _wordListByLength[length];
            await File.WriteAllLinesAsync(Path.Combine(outputFilePath, $"len{length:d2}.txt"), wordList);
        }
    }
    public async Task<IEnumerable<string>> Load(string inputFilePath) {
        if (!File.Exists(inputFilePath)) throw new FileNotFoundException();

        var loadedWords = await File.ReadAllLinesAsync(inputFilePath);
        _words = CleanseInputList(loadedWords).ToList();
        InitializeAfterLoad();
        GroupWordsByLength();
        return _words;
    }
    private static IEnumerable<string> CleanseInputList(IEnumerable<string> input) =>
        FilterDuplicate(input.Select(CleanseInput)).OrderBy(s => s);
    private static IEnumerable<string> FilterDuplicate(IEnumerable<string> input) => input.Distinct();
    private static string CleanseInput(string input) => input.Trim().ToLowerInvariant();
    private void InitializeAfterLoad() {
        var maxWordLength = _words.Any() ? _words.Max(s => s.Length) : 0;
        _wordListByLength = Enumerable.Range(1, maxWordLength).ToDictionary(i => i, _ => new List<string>());
    }
    private void GroupWordsByLength() {
        _words.ForEach(AddToWordListByLength);
    }
    private void AddToWordListByLength(string word) {
        _wordListByLength[word.Length].Add(word);
    }
}
