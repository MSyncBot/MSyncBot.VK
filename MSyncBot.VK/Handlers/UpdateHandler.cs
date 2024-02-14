using VkNet;
using VkNet.Model;

namespace MSyncBot.VK.Handlers;

public abstract class UpdateHandler
{
    public static async Task HandleUpdatesAsync(VkApi bot, BotsLongPollHistoryResponse updates)
    {
        foreach (var update in updates.Updates)
        {
            switch (update.Instance)
            {
                case MessageNew message:
                {
                    await new MessageHandler().HandleMessagesAsync(bot, message);
                    return;
                }
            }
        }
    }
}