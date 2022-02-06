namespace LittleToolCollection;

public class DistributionAnalyzer {
    private Dictionary<int, Dictionary<char, int>> charDistribution = new();
    private Dictionary<int, int> lengthDistribution = new();
    public DistributionReport AnalyzeDistribution(IEnumerable<string> words) {

        var wordsArr = words as string[] ?? words.ToArray();
        var totalWords = wordsArr.Length;
        var totalChars = wordsArr.Sum(s => s.Length);

        var maxWordLength = wordsArr.Any() ? wordsArr.Max(s => s.Length) : 0;
        charDistribution = Enumerable.Range(0, maxWordLength).ToDictionary(i => i, _ => InitializedCharDistribution());
        lengthDistribution = Enumerable.Range(0, maxWordLength).ToDictionary(i => i, _ => 0);

        lengthDistribution[0] = wordsArr.Length;
        foreach (var word in wordsArr) {
            lengthDistribution[word.Length] += 1;

            foreach (var c in word) {
                charDistribution[0][c] += 1;
                charDistribution[word.Length][c] += 1;
            }
        }

        return new DistributionReport(
        lengthDistribution.Select(
            distLength => {
                (var length, var count) = distLength;
                var totalCharOfLength = length * count;
                var wordRatioOfLength = count / (double)totalWords;
                return new LengthDistribution(
                    length, count, wordRatioOfLength,
                    charDistribution[length]
                        .Select(
                            distChar => {
                                (var c, var i) = distChar;
                                var charRatioOfLength = i / totalCharOfLength;
                                return new CharDistribution(c, i, charRatioOfLength);
                            })
                        .ToArray());
            }).ToArray()
        );
    }
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
    private static Dictionary<char, int> InitializedCharDistribution() => Alphabet.ToDictionary(c => c, _ => 0);
/*
{
    lengthDist: [
        {
            wordLength: 0,
            wordCount: 10,
            wordRatio: 0.01,

            charDist: [
                letter: a,
                letterCount: 10,
                letterRatio: 0.01
            ]
        },
        {
            length: 0,
            wordCount: 10,
            wordRatio: 0.01,
            charCount: 10,
            charRatio: 10,
            charDist: [
                letter: a,
                letterCount: 10,
                letterRatio: 0.01
            ]
        },
        {
            length: 2,
            wordCount: 10,
            wordRatio: 0.01,
            charCount: 10,
            charRatio: 10,
            charDist: [
                letter: a,
                letterCount: 10,
                letterRatio: 0.01
            ]
        }
    ]
}
 */
}
