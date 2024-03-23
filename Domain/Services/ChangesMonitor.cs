namespace Domain.Services;

public class ChangesMonitor
{
    public int ProcessedLines { get; private set; } = 0;
    public int CalculatedItems { get; private set; } = 0;
    public int WrittenResults { get; private set; } = 0;
    
    public int TotalLines { get; private set; } = 0;

    public void IncLines() => ProcessedLines++;
    public void IncItems() => CalculatedItems++;
    public void IncResults() => WrittenResults++;
    public void SetTotalLines(int count) => TotalLines = count;
}