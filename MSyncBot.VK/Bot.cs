using MLoggerService;
using MSyncBot.VK.Events;
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

        // Delegate for new incoming messages
        public delegate Task MessageNewEventHandler(MessageNewEventArgs args);
        public event MessageNewEventHandler? OnMessageNew;

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
                var longPollServer = await _api.Groups.GetLongPollServerAsync(groupId: 224017484); // my group (MSyncBot)
                var updates = await _api.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                {
                    Key = longPollServer.Key,
                    Server = longPollServer.Server,
                    Ts = longPollServer.Ts,
                    Wait = 25
                });

                // Handle updates
                foreach (var update in updates.Updates)
                {
                    if (update.Instance is MessageNew message)
                    {
                        // Call update incoming message
                        var messageNewEventArgs = new MessageNewEventArgs(message);
                        await OnMessageNew?.Invoke(messageNewEventArgs)!;
                        break;
                    }
                }
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