namespace ConsoleApp;

public class AppConfig
{
    public string InputFilePath { get; set; } = "input.csv";
    public string OutputFilePath { get; set; } = "output.csv";
    public int MaxDegreeOfParallelism { get; set; } = 3;
}