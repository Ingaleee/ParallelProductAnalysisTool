using System.Text.Json;

namespace ConsoleApp;

public static class Configurator
{
    public static AppConfig LoadOrCreateJson(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AppConfig>(json);
        }
        catch (FileNotFoundException)
        {
            var defaultConfig = new AppConfig();
            var json = JsonSerializer.Serialize(defaultConfig);
            File.WriteAllText(filePath, json);

            return defaultConfig;
        }
    }
}
