using System.Threading.Channels;

namespace Domain.Services;

public class FileReader : IFileReader
{
    private readonly ChannelWriter<string> _writer;
    private readonly ChangesMonitor _changes;

    public FileReader(ChannelWriter<string> writer, ChangesMonitor changes)
    {
        _writer = writer;
        _changes = changes;
    }

    public async Task ReadFileAsync(string path, CancellationToken token)
    {
        using var reader = new StreamReader(path);
        
        while (true)
        {
            token.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(token);
            if (string.IsNullOrEmpty(line))
            {
                break;
            } 
            
            await _writer.WriteAsync(line, token);
            _changes.IncLines();
        }

        _writer.Complete();
    }
}