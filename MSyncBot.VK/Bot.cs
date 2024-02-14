using MLoggerService;
using MSyncBot.VK.Handlers;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace MSyncBot.VK
{
    public class Bot
    {
        private readonly VkApi _bot;
        private readonly ulong _groupId;
        private readonly MLogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Bot(string accessToken, ulong groupId, MLogger logger)
        {
            _bot = new VkApi();
            _bot.Authorize(new ApiAuthParams
            {
                AccessToken = accessToken,
                Settings = Settings.All,
            });
            _groupId = groupId;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            _logger.LogSuccess("Bot started");
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
                _ = Task.Run(async () => await UpdateHandler.HandleUpdatesAsync(_bot, updates));
            }

            _logger.LogError("Bot stopped");
        }

        public async Task StopAsync()
        {
            _logger.LogProcess("Stopping bot...");
            _cancellationTokenSource.Cancel();
            await Task.Delay(1000); // Waiting 1 second until all operations shutting down
            _logger.LogSuccess("Bot stopped");
        }
    }
}