using MLoggerService;
using VkNet;
using VkNet.Model;

namespace MSyncBot.VK.Handlers;

public abstract class UpdateHandler
{
    public static void HandleUpdatesAsync(VkApi bot, BotsLongPollHistoryResponse updates, MLogger logger)
    {
        foreach (var update in updates.Updates)
        {
            switch (update.Instance)
            {
                case MessageNew message:
                {
                    _ = Task.Run(async () => await new MessageHandler().HandleMessagesAsync(bot, message, logger));
                    return;
                }
            }
        }
    }
}