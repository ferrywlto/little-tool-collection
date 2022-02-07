using System.Text;

namespace LittleToolCollection;

public class ConsoleOutput: IDistributionReportOutput {

    public void Output(DistributionReport report) {

        var sb = new StringBuilder();

        sb.AppendJoin(Environment.NewLine,
            report.LengthDistributions.Select(
                lengthDist => {
                    var innerSb = new StringBuilder();
                    innerSb.AppendLine();
                    innerSb.AppendJoin(Environment.NewLine, lengthDist.CharDistributions.Select(
                        charDist => "\t|-" + string.Format(
                            CharDistStr,
                            charDist.Letter,
                            charDist.Count,
                            charDist.Ratio)));

                    return string.Concat(string.Format(
                        LengthDistStr,
                        lengthDist.Length,
                        lengthDist.Count,
                        lengthDist.Ratio), innerSb);
                }));

        Console.WriteLine(sb.ToString());
    }

    private static string CharDistStr => string.Concat("Char ", DistStr);
    private static string LengthDistStr => string.Concat("Length ", DistStr);
    private static string DistStr => "{0}: Count = {1} ({2:P3})";
}
