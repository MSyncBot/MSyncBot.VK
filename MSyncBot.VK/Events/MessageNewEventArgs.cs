using VkNet.Model;

namespace MSyncBot.VK.Events;

public class MessageNewEventArgs(MessageNew message) : EventArgs
{
    public MessageNew Message { get; } = message;
}