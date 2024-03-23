namespace ConsoleApp;

public static class DataGenerator
{
    public static void Generate(string filePath, int rowCount)
    {
        var random = new Random();
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("id, prediction, stock");

        for (var i = 0; i < rowCount; i++)
        {
            var id = i + 1;
            var prediction = random.Next(1, 100);
            var stock = random.Next(0, 100);

            writer.WriteLine($"{id}, {prediction}, {stock}");
        }
    }
}
