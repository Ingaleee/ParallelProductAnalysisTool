namespace Domain.Services;

public interface IFileReader
{
    Task ReadFileAsync(string path, CancellationToken token);
}