using System.Threading.Channels;

namespace Domain.Services;

public class DemandCalculator : IDemandCalculator
{
    private readonly ChannelReader<string> _reader;
    private readonly ChannelWriter<(int id, int demand)> _writer;
    private readonly ChangesMonitor _changes;

    public DemandCalculator(ChannelReader<string> reader, ChannelWriter<(int id, int demand)> writer, ChangesMonitor changes)
    {
        _reader = reader;
        _writer = writer;
        _changes = changes;
    }

    public async Task CalculateDemandAsync(CancellationToken token)
    {
        await foreach (var line in _reader.ReadAllAsync(token))
        {
            token.ThrowIfCancellationRequested();
            var parts = line.Split(',');
            if (parts.Length != 3)
            {
                continue;
            } 
            
            if(!int.TryParse(parts[0], out var id))
            {
                continue;
            }

            if (!int.TryParse(parts[1], out var prediction))
            {
                continue;
            }
            
            if (!int.TryParse(parts[2], out int stock))
            {
                continue;
            }
            
            var demand = prediction - stock;
            await _writer.WriteAsync((id, Math.Max(demand, 0)), token);
            _changes.IncItems();
        }

        _writer.Complete();
    }
}