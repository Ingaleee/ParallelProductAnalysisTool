using System.Threading.Channels;
using Domain.Services;
using ConsoleApp;

public class Program
{
    public static async Task Main()
    {
        var configurations = Configurator.LoadOrCreateJson("appsettings.json");
        if (!File.Exists(configurations.InputFilePath))
        {
            DataGenerator.Generate(configurations.InputFilePath, 50000000);
        }

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var changesMonitor = new ChangesMonitor();

        var totalLines = File.ReadLines("input.csv").Count();
        changesMonitor.SetTotalLines(totalLines);

        var inputChannel = Channel.CreateUnbounded<string>();
        var outputChannel = Channel.CreateUnbounded<(int id, int demand)>();

        var reader = new FileReader(inputChannel.Writer, changesMonitor);
        var writer = new FileWriter(outputChannel.Reader, changesMonitor);
        var calculator = new DemandCalculator(inputChannel.Reader, outputChannel.Writer, changesMonitor);

        var progressTask = DisplayProgressAsync(changesMonitor, cancellationToken);

        var readerTask = reader.ReadFileAsync("input.csv", cancellationToken);

        var calculatorTask = Task.WhenAll(
            Enumerable.Range(0, configurations.MaxDegreeOfParallelism)
                .Select(_ => calculator.CalculateDemandAsync(cancellationToken))
                .ToArray());

        var writerTask = writer.WriteResultsAsync("output.csv", cancellationToken);

        Console.WriteLine("Down 'q' to cancel calculation");
        var cancelTask = WaitCancel(cancellationTokenSource);

        await Task.WhenAll(readerTask, calculatorTask, writerTask);

        cancellationTokenSource.Cancel();

        cancelTask.Dispose();
        progressTask.Dispose();

        Console.WriteLine("Calculation completed");
    }

    private static Task WaitCancel(CancellationTokenSource tokenSource)
    {
        while (true)
        {
            var key = Console.ReadKey();
            if (key.KeyChar.ToString().Equals("q", StringComparison.CurrentCultureIgnoreCase))
            {
                tokenSource.Cancel();
                break;
            }
        }

        return Task.CompletedTask;
    }

    private static async Task DisplayProgressAsync(ChangesMonitor changes, CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested();
            Console.WriteLine($"Lines processed: {changes.ProcessedLines}/{changes.TotalLines}, Items calculated: {changes.CalculatedItems}, Results written: {changes.WrittenResults}");
            await Task.Delay(100);
        }
    }
}

