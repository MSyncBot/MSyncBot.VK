using MLoggerService;

namespace MSyncBot.VK;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        var logger = new MLogger();
        logger.LogProcess("Initializing logger...");
        logger.LogSuccess("Logger successfully initialized.");

        logger.LogProcess("Initializing program configuration...");
        /*var configManager = new ConfigManager();
        var programConfig = new ProgramConfiguration();
        foreach (var property in typeof(ProgramConfiguration).GetProperties())
        {
            var propertyName = property.Name;
            var data = configManager.Get(propertyName);

            if (string.IsNullOrEmpty(data))
            {
                logger.LogInformation($"Enter value for {propertyName}:");
                data = Console.ReadLine();
                configManager.Set(programConfig);
            }

            property.SetValue(programConfig, Convert.ChangeType(data, property.PropertyType));
        }
        logger.LogSuccess("Program configuration has been initialized.");*/

        var bot = new Bot(programConfig.AccessToken, programConfig.GroupId, logger);
        _ = Task.Run(async () => await bot.StartAsync());
        
        Thread.Sleep(1500); // waiting for starting bot
        
        logger.LogInformation("Press any key to close program...");
        Console.ReadKey();
        bot.StopAsync().Wait();
    }
}