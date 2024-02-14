using VkNet.Model;

namespace MSyncBot.VK.Handlers;

public abstract class UpdateHandler
{
    public static async Task HandleUpdatesAsync(BotsLongPollHistoryResponse updates)
    {
        foreach (var update in updates.Updates)
        {
            switch (update.Instance)
            {
                case MessageNew message:
                {
                    // Call update incoming message
                    var messageNewEventArgs = new MessageNewEventArgs(message);
                    await OnMessageNew?.Invoke(messageNewEventArgs)!;
                    break;
                }
            }
        }
    }
}