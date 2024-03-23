namespace Domain.Services;

public interface IFileWriter
{
    Task WriteResultsAsync(string path, CancellationToken token);
}