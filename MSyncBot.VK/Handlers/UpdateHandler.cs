using MSyncBot.VK.Events;
using VkNet.Model;

namespace MSyncBot.VK.Handlers;

public class UpdateHandler
{
    // Delegate for new incoming messages
    public delegate Task MessageNewEventHandler(MessageNewEventArgs args);
    public event MessageNewEventHandler? OnMessageNew;
    
    public async Task HandleUpdatesAsync(BotsLongPollHistoryResponse updates)
    {
        // Handle updates
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