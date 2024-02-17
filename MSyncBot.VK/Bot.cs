using MLoggerService;
using MSyncBot.VK.Handlers;
using MSyncBot.VK.Handlers.Server;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace MSyncBot.VK
{
    public class Bot
    {
        private readonly VkApi _bot;
        private readonly ulong _groupId;
        private readonly CancellationTokenSource _cancellationTokenSource;
        
        public static MLogger Logger;
        public static ServerHandler? Server { get; private set; }

        public Bot(
            string accessToken,
            ulong groupId,
            string serverIp,
            int serverPort,
            MLogger logger)
        {
            _bot = new VkApi();
            _bot.Authorize(new ApiAuthParams
            {
                AccessToken = accessToken,
                Settings = Settings.All,
            });
            _groupId = groupId;
            
            Logger = logger;
            
            Server = new ServerHandler(serverIp, serverPort, _bot);
            
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            Logger.LogSuccess("Bot started.");
            
            // Connecting to the server
            _ = Task.Run(() => Server.ConnectAsync());
            
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Getting updates
                var longPollServer =
                    await _bot.Groups.GetLongPollServerAsync(groupId: _groupId);
                var updates = await _bot.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                {
                    Key = longPollServer.Key,
                    Server = longPollServer.Server,
                    Ts = longPollServer.Ts,
                    Wait = 25
                });

                // Handle updates
                _ = Task.Run(() => UpdateHandler.HandleUpdatesAsync(_bot, updates));
            }

            Logger.LogError("Bot stopped");
        }

        public async Task StopAsync()
        {
            Logger.LogProcess("Stopping bot...");
            await _cancellationTokenSource.CancelAsync();
            Logger.LogSuccess("Bot stopped");
        }
    }
}