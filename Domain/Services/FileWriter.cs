using System.Threading.Channels;

namespace Domain.Services;

public class FileWriter: IFileWriter
{
    private readonly ChannelReader<(int id, int demand)> _reader;
    private readonly ChangesMonitor _changes;

    public FileWriter(ChannelReader<(int id, int demand)> reader, ChangesMonitor changes)
    {
        _reader = reader;
        _changes = changes;
    }

    public async Task WriteResultsAsync(string path, CancellationToken token)
    {
        await using var writer = new StreamWriter(path);

        await foreach (var result in _reader.ReadAllAsync(token))
        {
            token.ThrowIfCancellationRequested();
            await writer.WriteLineAsync($"{result.id},{result.demand}");
            _changes.IncResults();
        }
    }
}