namespace LittleToolCollection;

public class DistributionAnalyzer {
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
    private Dictionary<int, Dictionary<char, int>> _charDistribution = new();
    private Dictionary<int, int> _lengthDistribution = new();
    public DistributionReport AnalyzeDistribution(IEnumerable<string> words) {
        var wordsArr = words as string[] ?? words.ToArray();
        InitializeDistribution(wordsArr);
        CountDistribution(wordsArr);
        return GetDistributionReport(wordsArr);
    }

    private void InitializeDistribution(string[] wordsArr) {
        var maxWordLength = wordsArr.Any() ? wordsArr.Max(s => s.Length) : 0;
        //the +1 here is to include global count at index 0
        _charDistribution = Enumerable.Range(0, maxWordLength+1).ToDictionary(i => i, _ => InitializedCharDistribution());
        _lengthDistribution = Enumerable.Range(0, maxWordLength+1).ToDictionary(i => i, _ => 0);
    }
    private void CountDistribution(IReadOnlyCollection<string> wordsArr) {
        _lengthDistribution[0] = wordsArr.Count;
        foreach (var word in wordsArr) {
            _lengthDistribution[word.Length] += 1;

            foreach (var c in word) {
                _charDistribution[0][c] += 1;
                _charDistribution[word.Length][c] += 1;
            }
        }
    }
    private DistributionReport GetDistributionReport(IReadOnlyCollection<string> wordsArr) {
        var totalWords = wordsArr.Count;
        return new DistributionReport(
            _lengthDistribution.Select(
                distLength => {
                    (var length, var count) = distLength;
                    var totalCharOfLength = length == 0 ? wordsArr.Sum(s => s.Length) : length * count;
                    var wordRatioOfLength = count / (double)totalWords;
                    return new LengthDistribution(
                        length, count, wordRatioOfLength,
                        _charDistribution[length]
                            .Select(
                                distChar => {
                                    (var c, var i) = distChar;
                                    var charRatioOfLength = totalCharOfLength == 0 ? 0 : i / (double)totalCharOfLength;
                                    return new CharDistribution(c, i, charRatioOfLength);
                                })
                            .ToArray());
                }).ToArray()
        );
    }
    private static Dictionary<char, int> InitializedCharDistribution() => Alphabet.ToDictionary(c => c, _ => 0);
}
