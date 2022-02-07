namespace LittleToolCollection;

public record LengthDistribution(
    int Length,
    int Count,
    double Ratio,
    CharDistribution[] CharDistributions
);
