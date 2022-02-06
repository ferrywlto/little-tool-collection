using System.Text;

namespace LittleToolCollection;

public class ConsoleOutput: IDistributionReportOutput {

    public void Output(DistributionReport2 report) {
        var distByChar = report.DistByChar;
        var distByLength = report.DistByLength;

        var sb = new StringBuilder();
        sb.AppendLine(PrintGlobalCharDistribution(distByChar));
        sb.AppendLine(PrintGlobalLengthDistribution(distByLength));

        Console.WriteLine(sb.ToString());
    }

    private static string PrintGlobalCharDistribution(GlobalCharDistribution charDistribution) {
        var sb = new StringBuilder();
        sb.AppendLine(string.Format(GlobalCharStr, charDistribution.TotalChar));

        sb.AppendJoin(
            Environment.NewLine,
            charDistribution.CharDistributions.Select(
                distribution => "\t|-"
                                + string.Format(
                                    CharDistStr,
                                    distribution.Letter,
                                    distribution.Count,
                                    distribution.Ratio)));

        return sb.ToString();
    }
    private static string PrintGlobalLengthDistribution(GlobalLengthDistribution globalLengthDistribution) {
        var sb = new StringBuilder();
        (var totalWord, var lengthDistributions) = globalLengthDistribution;
        sb.AppendLine(string.Format(GlobalLengthStr, totalWord));

        sb.AppendJoin(Environment.NewLine,
            lengthDistributions.Select(
                lengthDistribution => {
                    var innerSb = new StringBuilder();

                    innerSb.Append(
                        "\t|- "
                        + string.Format(
                            LengthDistStr,
                            lengthDistribution.Length,
                            lengthDistribution.Count,
                            lengthDistribution.Ratio));

                    innerSb.AppendLine($", Total chars = {lengthDistribution.TotalChar}");

                    innerSb.AppendJoin(
                        Environment.NewLine,
                        lengthDistribution.CharDistributions.Select(
                            charDistribution => "\t\t|-"
                                     + string.Format(
                                         CharDistStr,
                                         charDistribution.Letter,
                                         charDistribution.Count,
                                         charDistribution.Ratio)));

                    return innerSb.ToString();
                }));
        return sb.ToString();
    }
    private static string CharDistStr => string.Concat("Char ", DistStr);
    private static string LengthDistStr => string.Concat("Length ", DistStr);
    private static string DistStr => "{0}: Count = {1}({2:P2})";
    private static string GlobalCharStr => string.Concat(GlobalStr("char", "chars"), "{0}");
    private static string GlobalLengthStr => string.Concat(GlobalStr("length", "words"), "{0}");
    private static string GlobalStr(string p1, string p2) => $"Global {p1} distribution: Total {p2} = ";
}

public interface IDistributionReportOutput {
    public void Output(DistributionReport2 report);
}
