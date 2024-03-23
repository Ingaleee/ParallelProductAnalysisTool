namespace Domain.Services;

public interface IDemandCalculator
{
    Task CalculateDemandAsync(CancellationToken token);
}