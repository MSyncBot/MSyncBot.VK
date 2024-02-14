using MLoggerService;
using MSyncBot.VK.Handlers;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace MSyncBot.VK
{
    public class Bot
    {
        private readonly VkApi _api;
        private readonly MLogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Bot(string accessToken, MLogger logger)
        {
            _api = new VkApi();
            _api.Authorize(new ApiAuthParams
            {
                AccessToken = accessToken,
                Settings = Settings.All,
            });
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
                    await _api.Groups.GetLongPollServerAsync(groupId: 224017484); // my group (MSyncBot)
                var updates = await _api.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                {
                    Key = longPollServer.Key,
                    Server = longPollServer.Server,
                    Ts = longPollServer.Ts,
                    Wait = 25
                });

                _ = Task.Run(async () => await new UpdateHandler().HandleUpdatesAsync(updates));
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